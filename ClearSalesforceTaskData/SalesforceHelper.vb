Imports System.Data.SQLite
Imports SalesforceLib.Salesforce
''' <summary>
''' Salesforce 辅助模块
''' </summary>
Public Class SalesforceHelper

    Private Shared SforceServiceInstance As SforceService

    ''' <summary>
    ''' 初始化
    ''' </summary>
    Public Shared Sub Init()

        SforceServiceInstance = New SforceService With {
            .Timeout = 100 * 1000
        }

        Dim tmpLoginResult = SforceServiceInstance.login(AppSettingHelper.Instance.SalesforceUserName,
                                                         AppSettingHelper.Instance.SalesforcePassword)

        If tmpLoginResult.passwordExpired Then
            Throw New Exception("Salesforce 密码过期")
        End If

        ' 重定向到公司所属的服务器
        SforceServiceInstance.Url = tmpLoginResult.serverUrl
        SforceServiceInstance.SessionHeaderValue = New SessionHeader With {
            .sessionId = tmpLoginResult.sessionId
        }

    End Sub

    ''' <summary>
    ''' 注销登录
    ''' </summary>
    Public Shared Sub Logout()

        SforceServiceInstance.logout()

    End Sub

    Public Shared Sub ReadData(uie As Wangk.ResourceWPF.IBackgroundWorkEventArgs)

        SalesLocalDatabaseHelper.Clear()

        ReadTaskData(uie)

    End Sub

    Private Shared Sub ReadTaskData(uie As Wangk.ResourceWPF.IBackgroundWorkEventArgs)

        Dim tmpQueryResult = SforceServiceInstance.query($"
select
Id

from Task

where CreatedDate <= {AppSettingHelper.Instance.DeleteBeforeDate.Value:O}Z
")

        Dim readDone As Boolean = True

        If tmpQueryResult.size = 0 Then
            Exit Sub
        End If

        Dim sum = 0

        Do

            If uie.IsCancel Then
                Exit Do
            End If

            For Each item As Task In tmpQueryResult.records
                SalesLocalDatabaseHelper.Add(item)
            Next

            sum += tmpQueryResult.records.Length
            uie.Write($"已读取 : {sum:n0} / {tmpQueryResult.size:n0}")

            readDone = tmpQueryResult.done

            If Not readDone Then
                tmpQueryResult = SforceServiceInstance.queryMore(tmpQueryResult.queryLocator)
            End If

        Loop While Not readDone

    End Sub

    Public Shared Sub DeleteTaskData(uie As Wangk.ResourceWPF.IBackgroundWorkEventArgs)

        Using cmd As New SQLiteCommand(SalesLocalDatabaseHelper.DatabaseConnection)
            Dim recordsCount = 0
            cmd.CommandText = $"
select
count(0)

from Task"
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.Read Then
                    recordsCount = reader(0)
                End If
            End Using


            cmd.CommandText = $"
select
*

from Task
"
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                Dim index = 0

                Dim tmpList As New List(Of String)

                Do While reader.Read

                    If uie.IsCancel Then
                        Exit Do
                    End If

                    index += 1
                    uie.Write($"已删除 : {index:n0} / {recordsCount:n0}")

                    tmpList.Add(reader(0))

                    If tmpList.Count >= 200 Then
                        SforceServiceInstance.delete(tmpList.ToArray)
                        tmpList.Clear()
                    End If

                Loop

                If tmpList.Count > 0 Then
                    SforceServiceInstance.delete(tmpList.ToArray)
                End If

            End Using
        End Using

    End Sub

End Class
