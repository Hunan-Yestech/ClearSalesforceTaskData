''' <summary>
''' 全局配置辅助类
''' </summary>
Public Class AppSettingHelper
    Private Sub New()
    End Sub

#Region "配置参数"
    ''' <summary>
    ''' 实例
    ''' </summary>
    Private Shared _instance As AppSettingHelper
    ''' <summary>
    ''' 获取实例
    ''' </summary>
    Public Shared ReadOnly Property Instance As AppSettingHelper
        Get
            If _instance Is Nothing Then
                _instance = New AppSettingHelper
            End If

            Return _instance
        End Get
    End Property
#End Region

    ''' <summary>
    ''' Salesforce 用户名
    ''' </summary>
    Public SalesforceUserName As String

    ''' <summary>
    ''' Salesforce 密码
    ''' </summary>
    Public SalesforcePassword As String

    ''' <summary>
    ''' Salesforce 本地数据库文件路径
    ''' </summary>
    Public Shared SalesforceLocalDatabaseFilePath As String = $".\SalesLocalDatabase.db"

    ''' <summary>
    ''' Salesforce 本地数据库连接字符串
    ''' </summary>
    Public Shared ReadOnly Property SalesforceLocalDatabaseConnection As String
        Get
            Return $"data source= {SalesforceLocalDatabaseFilePath}"
        End Get
    End Property

    ''' <summary>
    ''' 删除日期之前的数据
    ''' </summary>
    Public DeleteBeforeDate As Date? = New Date(2021, 1, 1)

End Class
