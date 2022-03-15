Imports System.Data
Imports System.Data.SQLite
Imports SalesforceLib.Salesforce

''' <summary>
''' 销售本地数据库辅助模块
''' </summary>
Public Class SalesLocalDatabaseHelper

    Private Shared _DatabaseConnection As SQLite.SQLiteConnection
    Public Shared ReadOnly Property DatabaseConnection() As SQLiteConnection
        Get
            ' 创建数据库文件
            If Not IO.File.Exists(AppSettingHelper.SalesforceLocalDatabaseFilePath) Then
                SQLiteConnection.CreateFile(AppSettingHelper.SalesforceLocalDatabaseFilePath)
            End If

            ' 连接数据库
            If _DatabaseConnection Is Nothing Then

                _DatabaseConnection = New SQLite.SQLiteConnection With {
                    .ConnectionString = AppSettingHelper.SalesforceLocalDatabaseConnection
                }
                _DatabaseConnection.Open()

                Init()

            End If

            Return _DatabaseConnection
        End Get
    End Property

    Public Shared Sub Close()

        Try
            _DatabaseConnection?.Close()

        Catch ex As Exception

        Finally
            _DatabaseConnection = Nothing
        End Try

    End Sub

#Region "清空数据库"
    ''' <summary>
    ''' 清空数据库
    ''' </summary>
    Public Shared Sub Clear()

        Close()

        If IO.File.Exists(AppSettingHelper.SalesforceLocalDatabaseFilePath) Then
            IO.File.Delete(AppSettingHelper.SalesforceLocalDatabaseFilePath)
        End If

    End Sub
#End Region

#Region "初始化数据库"
    ''' <summary>
    ''' 初始化数据库
    ''' </summary>
    Public Shared Sub Init()

        Using cmd As New SQLite.SQLiteCommand(DatabaseConnection)
            cmd.CommandText = "
--关闭同步
PRAGMA synchronous = OFF;
--不记录日志
PRAGMA journal_mode = OFF;"

            cmd.ExecuteNonQuery()

            CreateTask(cmd)

        End Using

    End Sub
#End Region

#Region "Task"
    Private Shared Sub CreateTask(cmd As SQLite.SQLiteCommand)

        Try

            cmd.CommandText = $"
CREATE TABLE Task (
    Id  STRING
);
"
            cmd.ExecuteNonQuery()

        Catch ex As Exception
        End Try

    End Sub

    Public Shared Sub Add(value As Task)

        Using cmd As New SQLiteCommand(DatabaseConnection) With {
                .CommandText = "insert into
Task 
values(
@Id
)"
        }
            cmd.Parameters.Add(New SQLiteParameter("@Id", DbType.String) With {.Value = value.Id})

            cmd.ExecuteNonQuery()
        End Using

    End Sub

#End Region

End Class
