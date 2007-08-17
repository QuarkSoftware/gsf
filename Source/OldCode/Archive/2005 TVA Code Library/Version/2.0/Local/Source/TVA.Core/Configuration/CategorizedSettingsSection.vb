'*******************************************************************************************************
'  TVA.Configuration.CategorizedSettingsSection.vb - Categorized Settings Section
'  Copyright � 2006 - TVA, all rights reserved - Gbtc
'
'  Build Environment: VB.NET, Visual Studio 2005
'  Primary Developer: Pinal C. Patel, Operations Data Architecture [TVA]
'      Office: COO - TRNS/PWR ELEC SYS O, CHATTANOOGA, TN - MR 2W-C
'       Phone: 423/751-2250
'       Email: pcpatel@tva.gov
'
'  Code Modification History:
'  -----------------------------------------------------------------------------------------------------
'  04/11/2006 - Pinal C. Patel
'       Generated original version of source code
'
'*******************************************************************************************************

Imports System.Xml
Imports System.Configuration

Namespace Configuration

    ''' <summary>
    ''' Represents a section in the configuration with one or more 
    ''' TVA.Configuration.CategorizedSettingsCollection.
    ''' </summary>
    Public Class CategorizedSettingsSection
        Inherits ConfigurationSection

        ''' <summary>
        ''' Gets the TVA.Configuration.CategorizedSettingsCollection for the specified category name.
        ''' </summary>
        ''' <param name="name">The name of the TVA.Configuration.CategorizedSettingsCollection to return.</param>
        ''' <returns>The TVA.Configuration.CategorizedSettingsCollection with the specified name; otherwise null.</returns>
        Default Public ReadOnly Property Category(ByVal name As String) As CategorizedSettingsElementCollection
            Get
                ' We will add the requested category to the default properties collection, so that when 
                ' the settings are saved off to the config file, all of the categories under which 
                ' settings may be saved are processed and saved to the config file. This is essentially 
                ' doing what marking a property with the <ConfigurationProperty()> attribute does.
                ' Make the first letter of category name lower case, if not already.
                Dim nameChars() As Char = name.ToCharArray()
                nameChars(0) = Char.ToLower(nameChars(0))
                ' Do not allow spaces in the category name, so that underlying .Net configuration API does not 
                ' break.
                name = TVA.Text.Common.RemoveWhiteSpace(Convert.ToString(nameChars))
                Dim configProperty As New ConfigurationProperty(name, GetType(CategorizedSettingsElementCollection))

                MyBase.Properties.Add(configProperty)
                Dim settingsCategory As CategorizedSettingsElementCollection = Nothing
                If MyBase.Item(configProperty) IsNot Nothing Then
                    settingsCategory = DirectCast(MyBase.Item(configProperty), CategorizedSettingsElementCollection)
                End If
                Return settingsCategory
            End Get
        End Property

        ''' <summary>
        ''' Gets the "general" category under the "categorizedSettings" of the configuration file.
        ''' </summary>
        ''' <returns>The TVA.Configuration.CategorizedSettingsCollection of the "general" category.</returns>
        Public ReadOnly Property General() As CategorizedSettingsElementCollection
            Get
                Return Category("general")
            End Get
        End Property

        Protected Overrides Sub DeserializeSection(ByVal reader As System.Xml.XmlReader)

            Dim configSectionStream As New System.IO.MemoryStream()
            Dim configSection As New XmlDocument()
            configSection.Load(reader)
            configSection.Save(configSectionStream)
            ' Adds all the categories that are under the categorizedSettings section of the configuration file
            ' to the property collection. Again, this is essentially doing what marking a property with the
            ' <ConfigurationProperty()> attribute does. If this is not done, then an exception will be raised
            ' when the category elements are being deserialized.
            For Each category As XmlNode In configSection.DocumentElement.SelectNodes("*")
                MyBase.Properties.Add(New ConfigurationProperty(category.Name(), GetType(CategorizedSettingsElementCollection)))
            Next
            configSectionStream.Seek(0, System.IO.SeekOrigin.Begin)

            MyBase.DeserializeSection(XmlReader.Create(configSectionStream))

        End Sub

    End Class

End Namespace
