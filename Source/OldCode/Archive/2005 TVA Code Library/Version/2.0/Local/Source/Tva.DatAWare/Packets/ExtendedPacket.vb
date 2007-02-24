Imports System.Text

Namespace Packets

    Public Class ExtendedPacket
        Inherits PacketBase

        ' *******************************************************
        ' *                 Packet structure                    *
        ' *******************************************************
        ' * # Of Bytes  Byte Index  Data Type   Description     *
        ' * ----------  ----------  ----------  -----------     *
        ' * 2           0-1         Int16       Packet ID       *
        ' * 4           2-5         Int32       Database index  *
        ' * 2           6-7         Int16       Year            *
        ' * 1           8           Byte        Month           *
        ' * 1           9           Byte        Day             *
        ' * 1           10          Byte        Hour            *
        ' * 1           11          Byte        Minute          *
        ' * 1           12          Byte        Second          *
        ' * 1           13          Byte        Quality         *
        ' * 2           14-15       Int16       Msec            *
        ' * 2           16-17       Int16       GMT Offset      *
        ' * 4           18-21       Single      Value           *
        ' *******************************************************

#Region " Member Declaration "

        Private m_index As Integer
        Private m_year As Short
        Private m_month As Byte
        Private m_day As Byte
        Private m_hour As Byte
        Private m_minute As Byte
        Private m_second As Byte
        Private m_quality As Byte
        Private m_millisecond As Short
        Private m_gmtOffset As Short
        Private m_value As Single

#End Region

        Public Shadows Const TypeID As Short = 2
        Public Shadows Const BinaryLength As Integer = 22

        Public Sub New()

            MyBase.New()
            MyBase.ActionType = PacketActionType.SaveAndReply
            MyBase.SaveLocation = PacketSaveLocation.ArchiveFile

        End Sub

        Public Sub New(ByVal binaryImage As Byte())

            MyClass.New(binaryImage, 0)

        End Sub

        Public Sub New(ByVal binaryImage As Byte(), ByVal startIndex As Integer)

            MyClass.New()

            If (binaryImage.Length - startIndex) >= BinaryLength Then
                ' We have a binary image of valid size.
                Dim packetTypeID As Short = BitConverter.ToInt16(binaryImage, startIndex)
                If packetTypeID = TypeID Then
                    ' We have a binary image with the correct type ID.
                    m_index = BitConverter.ToInt32(binaryImage, startIndex + 2)
                    m_year = BitConverter.ToInt16(binaryImage, startIndex + 6)
                    m_month = binaryImage(startIndex + 8)
                    m_day = binaryImage(startIndex + 9)
                    m_hour = binaryImage(startIndex + 10)
                    m_minute = binaryImage(startIndex + 11)
                    m_second = binaryImage(startIndex + 12)
                    m_quality = binaryImage(startIndex + 13)
                    m_millisecond = BitConverter.ToInt16(binaryImage, startIndex + 14)
                    m_gmtOffset = BitConverter.ToInt16(binaryImage, startIndex + 16)
                    m_value = BitConverter.ToSingle(binaryImage, startIndex + 18)
                Else
                    Throw New ArgumentException(String.Format("Unexpected packet type ID {0}. Expected packet type ID {1}.", packetTypeID, TypeID))
                End If
            Else
                Throw New ArgumentException(String.Format("Binary image smaller than expected. Expected binary image size {0}.", BinaryLength))
            End If

        End Sub

        Public Sub New(ByVal index As Integer, ByVal year As Short, ByVal month As Byte, ByVal day As Byte, _
                ByVal hour As Byte, ByVal minute As Byte, ByVal second As Byte, ByVal quality As Byte, _
                ByVal millisecond As Short, ByVal gmtOffset As Short, ByVal value As Single)

            MyClass.New()

            m_index = index
            m_year = year
            m_month = month
            m_day = day
            m_hour = hour
            m_minute = minute
            m_second = second
            m_quality = quality
            m_millisecond = millisecond
            m_gmtOffset = gmtOffset
            m_value = value

        End Sub

        Public Property Index() As Integer
            Get
                Return m_index
            End Get
            Set(ByVal value As Integer)
                m_index = value
            End Set
        End Property

        Public Property Year() As Short
            Get
                Return m_year
            End Get
            Set(ByVal value As Short)
                m_year = value
            End Set
        End Property

        Public Property Month() As Byte
            Get
                Return m_month
            End Get
            Set(ByVal value As Byte)
                m_month = value
            End Set
        End Property

        Public Property Day() As Byte
            Get
                Return m_day
            End Get
            Set(ByVal value As Byte)
                m_day = value
            End Set
        End Property

        Public Property Hour() As Byte
            Get
                Return m_hour
            End Get
            Set(ByVal value As Byte)
                m_hour = value
            End Set
        End Property

        Public Property Minute() As Byte
            Get
                Return m_minute
            End Get
            Set(ByVal value As Byte)
                m_minute = value
            End Set
        End Property

        Public Property Second() As Byte
            Get
                Return m_second
            End Get
            Set(ByVal value As Byte)
                m_second = value
            End Set
        End Property

        Public Property Quality() As Byte
            Get
                Return m_quality
            End Get
            Set(ByVal value As Byte)
                m_quality = value
            End Set
        End Property

        Public Property Millisecond() As Short
            Get
                Return m_millisecond
            End Get
            Set(ByVal value As Short)
                m_millisecond = value
            End Set
        End Property

        Public Property GMTOffset() As Short
            Get
                Return m_gmtOffset
            End Get
            Set(ByVal value As Short)
                m_gmtOffset = value
            End Set
        End Property

        Public Property Value() As Single
            Get
                Return m_value
            End Get
            Set(ByVal value As Single)
                m_value = value
            End Set
        End Property

        Public Overrides Function GetReplyData() As Byte()

            Return Encoding.ASCII.GetBytes("ACK")

        End Function

        Public Overrides Function GetSaveData() As Byte()

            Dim timestamp As New System.DateTime(Convert.ToInt32(m_year), _
                Convert.ToInt32(m_month), Convert.ToInt32(m_day), _
                Convert.ToInt32(m_hour) + Convert.ToInt32(m_gmtOffset), _
                Convert.ToInt32(m_minute), Convert.ToInt32(m_second), _
                Convert.ToInt32(m_millisecond), DateTimeKind.Utc)

            'Return New ExtendedPointData(timestamp, m_value, m_quality).BinaryImage
            Return Nothing

        End Function

        Public Shared Shadows Function TryParse(ByVal binaryImage As Byte(), ByRef packets As List(Of IPacket)) As Boolean

            If binaryImage IsNot Nothing Then
                If binaryImage.Length >= BinaryLength Then
                    packets = New List(Of IPacket)()
                    For i As Integer = 0 To binaryImage.Length - 1 Step BinaryLength
                        Try
                            packets.Add(New StandardPacket(binaryImage, i))
                        Catch ex As Exception
                            ' Its likely that we encounter an exception here if the entire buffer is not well formed.
                        End Try
                    Next
                    Return True
                End If
            End If

            Return False

        End Function

    End Class

End Namespace