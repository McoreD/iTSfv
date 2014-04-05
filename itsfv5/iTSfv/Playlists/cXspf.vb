Imports System.Xml.Serialization

' Code converted from C# to VB.NET
' Source: http://www.codeproject.com/useritems/ASPNET_MP3_Player.asp
' Changed date data type from Date to String to serialize 

<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")> _
<System.SerializableAttribute()> _
<System.Diagnostics.DebuggerStepThroughAttribute()> _
<System.ComponentModel.DesignerCategoryAttribute("code")> _
<System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://xspf.org/ns/0/")> _
<System.Xml.Serialization.XmlRootAttribute("playlist", [Namespace]:="http://xspf.org/ns/0/", IsNullable:=False)> _
Partial Public Class cXspf

    Private titleField As String
    Private creatorField As String
    Private annotationField As String
    Private infoField As String
    Private locationField As String
    Private identifierField As String
    Private imageField As String
    Private dateField As String
    Private dateFieldSpecified As Boolean
    Private licenseField As String
    Private attributionField As AttributionType
    Private linkField As LinkType()
    Private metaField As MetaType()
    Private extensionField As ExtensionType()
    Private trackListField As TrackType()
    Private versionField As String

    ''' <remarks/>
    Public Property title() As String
        Get
            Return Me.titleField
        End Get
        Set(ByVal value As String)
            Me.titleField = value
        End Set
    End Property

    ''' <remarks/>
    Public Property creator() As String
        Get
            Return Me.creatorField
        End Get
        Set(ByVal value As String)
            Me.creatorField = value
        End Set
    End Property

    ''' <remarks/>
    Public Property annotation() As String
        Get
            Return Me.annotationField
        End Get
        Set(ByVal value As String)
            Me.annotationField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="anyURI")> _
    Public Property info() As String
        Get
            Return Me.infoField
        End Get
        Set(ByVal value As String)
            Me.infoField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="anyURI")> _
    Public Property location() As String
        Get
            Return Me.locationField
        End Get
        Set(ByVal value As String)
            Me.locationField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="anyURI")> _
    Public Property identifier() As String
        Get
            Return Me.identifierField
        End Get
        Set(ByVal value As String)
            Me.identifierField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="anyURI")> _
    Public Property image() As String
        Get
            Return Me.imageField
        End Get
        Set(ByVal value As String)
            Me.imageField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("date")> _
    Public Property dateString() As String
        Get
            Return Me.dateField
        End Get
        Set(ByVal value As String)
            Me.dateField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlIgnoreAttribute()> _
    Public Property dateSpecified() As Boolean
        Get
            Return Me.dateFieldSpecified
        End Get
        Set(ByVal value As Boolean)
            Me.dateFieldSpecified = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="anyURI")> _
    Public Property license() As String
        Get
            Return Me.licenseField
        End Get
        Set(ByVal value As String)
            Me.licenseField = value
        End Set
    End Property

    ''' <remarks/>
    Public Property attribution() As AttributionType
        Get
            Return Me.attributionField
        End Get
        Set(ByVal value As AttributionType)
            Me.attributionField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("link")> _
    Public Property link() As LinkType()
        Get
            Return Me.linkField
        End Get
        Set(ByVal value As LinkType())
            Me.linkField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("meta")> _
    Public Property meta() As MetaType()
        Get
            Return Me.metaField
        End Get
        Set(ByVal value As MetaType())
            Me.metaField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("extension")> _
    Public Property extension() As ExtensionType()
        Get
            Return Me.extensionField
        End Get
        Set(ByVal value As ExtensionType())
            Me.extensionField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlArrayItemAttribute("track", IsNullable:=False)> _
    Public Property trackList() As TrackType()
        Get
            Return Me.trackListField
        End Get
        Set(ByVal value As TrackType())
            Me.trackListField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()> _
    Public Property version() As String
        Get
            Return Me.versionField
        End Get
        Set(ByVal value As String)
            Me.versionField = value
        End Set
    End Property

End Class

''' <remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")> _
<System.SerializableAttribute()> _
<System.Diagnostics.DebuggerStepThroughAttribute()> _
<System.ComponentModel.DesignerCategoryAttribute("code")> _
<System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://xspf.org/ns/0/")> _
Partial Public Class AttributionType

    Private itemsField As String()

    Private itemsElementNameField As ItemsChoiceType()

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("identifier", GetType(String), DataType:="anyURI")> _
    <System.Xml.Serialization.XmlElementAttribute("location", GetType(String), DataType:="anyURI")> _
    <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")> _
    Public Property Items() As String()
        Get
            Return Me.itemsField
        End Get
        Set(ByVal value As String())
            Me.itemsField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")> _
    <System.Xml.Serialization.XmlIgnoreAttribute()> _
    Public Property ItemsElementName() As ItemsChoiceType()
        Get
            Return Me.itemsElementNameField
        End Get
        Set(ByVal value As ItemsChoiceType())
            Me.itemsElementNameField = value
        End Set
    End Property
End Class

''' <remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")> _
<System.SerializableAttribute()> _
<System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://xspf.org/ns/0/", IncludeInSchema:=False)> _
Public Enum ItemsChoiceType

    ''' <remarks/>
    identifier

    ''' <remarks/>
    location
End Enum

''' <remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")> _
<System.SerializableAttribute()> _
<System.Diagnostics.DebuggerStepThroughAttribute()> _
<System.ComponentModel.DesignerCategoryAttribute("code")> _
<System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://xspf.org/ns/0/")> _
Partial Public Class ExtensionType

    Private textField As String()

    ''' <remarks/>
    <System.Xml.Serialization.XmlTextAttribute()> _
    Public Property Text() As String()
        Get
            Return Me.textField
        End Get
        Set(ByVal value As String())
            Me.textField = value
        End Set
    End Property
End Class

''' <remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")> _
<System.SerializableAttribute()> _
<System.Diagnostics.DebuggerStepThroughAttribute()> _
<System.ComponentModel.DesignerCategoryAttribute("code")> _
<System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://xspf.org/ns/0/")> _
Partial Public Class TrackType

    Private locationField As String()
    Private identifierField As String()
    Private titleField As String
    Private creatorField As String
    Private annotationField As String
    Private infoField As String
    Private imageField As String
    Private albumField As String
    Private trackNumField As String
    Private durationField As String
    Private linkField As LinkType()
    Private metaField As MetaType()
    Private extensionField As ExtensionType()

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("location", DataType:="anyURI")> _
    Public Property location() As String()
        Get
            Return Me.locationField
        End Get
        Set(ByVal value As String())
            Me.locationField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("identifier", DataType:="anyURI")> _
    Public Property identifier() As String()
        Get
            Return Me.identifierField
        End Get
        Set(ByVal value As String())
            Me.identifierField = value
        End Set
    End Property

    ''' <remarks/>
    Public Property title() As String
        Get
            Return Me.titleField
        End Get
        Set(ByVal value As String)
            Me.titleField = value
        End Set
    End Property

    ''' <remarks/>
    Public Property creator() As String
        Get
            Return Me.creatorField
        End Get
        Set(ByVal value As String)
            Me.creatorField = value
        End Set
    End Property

    ''' <remarks/>
    Public Property annotation() As String
        Get
            Return Me.annotationField
        End Get
        Set(ByVal value As String)
            Me.annotationField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="anyURI")> _
    Public Property info() As String
        Get
            Return Me.infoField
        End Get
        Set(ByVal value As String)
            Me.infoField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="anyURI")> _
    Public Property image() As String
        Get
            Return Me.imageField
        End Get
        Set(ByVal value As String)
            Me.imageField = value
        End Set
    End Property

    ''' <remarks/>
    Public Property album() As String
        Get
            Return Me.albumField
        End Get
        Set(ByVal value As String)
            Me.albumField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="nonNegativeInteger")> _
    Public Property trackNum() As String
        Get
            Return Me.trackNumField
        End Get
        Set(ByVal value As String)
            Me.trackNumField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute(DataType:="nonNegativeInteger")> _
    Public Property duration() As String
        Get
            Return Me.durationField
        End Get
        Set(ByVal value As String)
            Me.durationField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("link")> _
    Public Property link() As LinkType()
        Get
            Return Me.linkField
        End Get
        Set(ByVal value As LinkType())
            Me.linkField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("meta")> _
    Public Property meta() As MetaType()
        Get
            Return Me.metaField
        End Get
        Set(ByVal value As MetaType())
            Me.metaField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlElementAttribute("extension")> _
    Public Property extension() As ExtensionType()
        Get
            Return Me.extensionField
        End Get
        Set(ByVal value As ExtensionType())
            Me.extensionField = value
        End Set
    End Property
End Class

''' <remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")> _
<System.SerializableAttribute()> _
<System.Diagnostics.DebuggerStepThroughAttribute()> _
<System.ComponentModel.DesignerCategoryAttribute("code")> _
<System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://xspf.org/ns/0/")> _
Partial Public Class LinkType

    Private relField As String
    Private valueField As String

    ''' <remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")> _
    Public Property rel() As String
        Get
            Return Me.relField
        End Get
        Set(ByVal value As String)
            Me.relField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlTextAttribute(DataType:="anyURI")> _
    Public Property Value() As String
        Get
            Return Me.valueField
        End Get
        Set(ByVal value As String)
            Me.valueField = value
        End Set
    End Property
End Class

''' <remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")> _
<System.SerializableAttribute()> _
<System.Diagnostics.DebuggerStepThroughAttribute()> _
<System.ComponentModel.DesignerCategoryAttribute("code")> _
<System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://xspf.org/ns/0/")> _
Partial Public Class MetaType

    Private relField As String
    Private valueField As String

    ''' <remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")> _
    Public Property rel() As String
        Get
            Return Me.relField
        End Get
        Set(ByVal value As String)
            Me.relField = value
        End Set
    End Property

    ''' <remarks/>
    <System.Xml.Serialization.XmlTextAttribute()> _
    Public Property Value() As String
        Get
            Return Me.valueField
        End Get
        Set(ByVal value As String)
            Me.valueField = value
        End Set
    End Property
End Class
