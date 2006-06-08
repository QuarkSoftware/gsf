' 06-02-06

Option Strict On

Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports Tva.Common
Imports Tva.Data.Transport.Common

Namespace Data.Transport

    Public Class TcpClient

        Private m_connectionStringData As Hashtable
        Private m_tcpClient As Socket

        Public Sub New(ByVal connectionString As String)
            MyClass.New()
            MyBase.ConnectionString = connectionString
        End Sub

        Public Overrides Sub Connect()

            If MyBase.Enabled() AndAlso Not MyBase.IsConnected() Then
                With New Thread(AddressOf ConnectToServer)
                    .Start()
                End With
            End If

        End Sub

        Public Overrides Sub Disconnect()

            If MyBase.Enabled() AndAlso MyBase.IsConnected() Then
                If m_tcpClient IsNot Nothing Then m_tcpClient.Close()
            End If

        End Sub

        Public Overrides Sub Send(ByVal data() As Byte)

            If MyBase.Enabled() AndAlso MyBase.IsConnected() Then
                If data IsNot Nothing Then
                    MyBase.OnSendBegin(data)
                    m_tcpClient.Send(data)
                    MyBase.OnSendComplete(data)
                Else
                    Throw New ArgumentNullException("data")
                End If
            End If

        End Sub

        Protected Overrides Function ValidConnectionString(ByVal connectionString As String) As Boolean

            If Not String.IsNullOrEmpty(connectionString) Then
                m_connectionStringData = ParseInitializationString(connectionString)
                If m_connectionStringData.Contains("SERVER") AndAlso _
                        m_connectionStringData.Contains("PORT") AndAlso _
                        Dns.GetHostEntry(Convert.ToString(m_connectionStringData("SERVER"))) IsNot Nothing AndAlso _
                        ValidPortNumber(Convert.ToString(m_connectionStringData("PORT"))) Then
                    Return True
                Else
                    ' Connection string is not in the expected format.
                    With New StringBuilder()
                        .Append("Connection string must be in the following format:")
                        .Append(Environment.NewLine())
                        .Append("   Server=<Server name or IP>;Port=<Port Number>")
                        Throw New ArgumentException(.ToString())
                    End With
                End If
            Else
                Throw New ArgumentNullException()
            End If

        End Function

        Private Sub ConnectToServer()

            MyBase.OnConnecting(EventArgs.Empty)
            Dim connectionAttempts As Integer = 0
            Do While (MyBase.MaximumConnectionAttempts() = 0) OrElse _
                    (connectionAttempts < MyBase.MaximumConnectionAttempts())
                Try
                    m_tcpClient = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    m_tcpClient.Bind(New IPEndPoint(IPAddress.Any, 0))
                    m_tcpClient.Connect(GetIpEndPoint(Convert.ToString(m_connectionStringData("SERVER")), _
                        Convert.ToInt32(m_connectionStringData("PORT"))))
                    With New Thread(AddressOf ReceiveServerData)
                        .Start()
                    End With

                    Exit Do ' Client successfully connected to the server.
                Catch ex As Exception
                    m_tcpClient = Nothing
                Finally
                    connectionAttempts += 1
                End Try
            Loop

        End Sub

        Private Sub ReceiveServerData()

            Try
                MyBase.OnConnected(EventArgs.Empty)

                Do While True
                    Dim receivedData() As Byte = CreateArray(Of Byte)(MyBase.ReceiveBufferSize())
                    m_tcpClient.Receive(receivedData)
                    MyBase.OnReceivedData(receivedData)
                Loop
            Catch ex As Exception

            Finally
                If m_tcpClient IsNot Nothing Then
                    m_tcpClient.Close()
                    m_tcpClient = Nothing
                End If
                MyBase.OnDisconnected(EventArgs.Empty)
            End Try

        End Sub

    End Class

End Namespace