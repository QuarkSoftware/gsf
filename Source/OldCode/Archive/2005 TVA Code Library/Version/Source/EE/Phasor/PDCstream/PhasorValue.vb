'*******************************************************************************************************
'  PhasorValue.vb - PDCstream Phasor value
'  Copyright � 2005 - TVA, all rights reserved - Gbtc
'
'  Build Environment: VB.NET, Visual Studio 2003
'  Primary Developer: James R Carroll, System Analyst [TVA]
'      Office: COO - TRNS/PWR ELEC SYS O, CHATTANOOGA, TN - MR 2W-C
'       Phone: 423/751-2827
'       Email: jrcarrol@tva.gov
'
'  Code Modification History:
'  -----------------------------------------------------------------------------------------------------
'  11/12/2004 - James R Carroll
'       Initial version of source generated
'
'*******************************************************************************************************

Imports TVA.Interop
Imports TVA.Shared.Math

Namespace EE.Phasor.PDCstream

    Public Class PhasorValue

        Inherits PhasorValueBase

        Public Overloads Shared Function CreateFromPolarValues(ByVal parent As IDataCell, ByVal phasorDefinition As IPhasorDefinition, ByVal angle As Double, ByVal magnitude As Double) As PhasorValue

            Return PhasorValueBase.CreateFromPolarValues(GetType(PhasorValue), parent, phasorDefinition, angle, magnitude)

        End Function

        Public Overloads Shared Function CreateFromRectangularValues(ByVal parent As IDataCell, ByVal phasorDefinition As IPhasorDefinition, ByVal real As Double, ByVal imaginary As Double) As PhasorValue

            Return PhasorValueBase.CreateFromRectangularValues(GetType(PhasorValue), parent, phasorDefinition, real, imaginary)

        End Function

        Public Overloads Shared Function CreateFromUnscaledRectangularValues(ByVal parent As IDataCell, ByVal phasorDefinition As IPhasorDefinition, ByVal real As Int16, ByVal imaginary As Int16) As PhasorValue

            Return PhasorValueBase.CreateFromUnscaledRectangularValues(GetType(PhasorValue), parent, phasorDefinition, real, imaginary)

        End Function

        Public Sub New(ByVal parent As IDataCell, ByVal phasorDefinition As IPhasorDefinition, ByVal real As Double, ByVal imaginary As Double)

            MyBase.New(parent, phasorDefinition, real, imaginary)

        End Sub

        Public Sub New(ByVal parent As IDataCell, ByVal phasorDefinition As IPhasorDefinition, ByVal unscaledReal As Int16, ByVal unscaledImaginary As Int16)

            MyBase.New(parent, phasorDefinition, unscaledReal, unscaledImaginary)

        End Sub

        Public Sub New(ByVal parent As IDataCell, ByVal phasorDefinition As IPhasorDefinition, ByVal binaryImage As Byte(), ByVal startIndex As Integer)

            MyBase.New(parent, phasorDefinition, binaryImage, startIndex)

        End Sub

        Public Sub New(ByVal phasorValue As IPhasorValue)

            MyBase.New(phasorValue)

        End Sub

        Public Overrides ReadOnly Property InheritedType() As System.Type
            Get
                Return Me.GetType()
            End Get
        End Property

        Public Shared Function CalculateBinaryLength(ByVal definition As PhasorDefinition) As Int16

            ' The phasor definition will determine the binary length based on data format
            Return (New PhasorValue(Nothing, definition, 0, 0)).BinaryLength

        End Function

    End Class

End Namespace