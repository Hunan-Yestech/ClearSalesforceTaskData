Class MainWindow

    Private Sub SearchData(sender As Object, e As RoutedEventArgs)

        AppSettingHelper.Instance.SalesforceUserName = SalesforceUserName.Text
        AppSettingHelper.Instance.SalesforcePassword = SalesforcePassword.Password
        AppSettingHelper.Instance.DeleteBeforeDate = DeleteBeforeDate.SelectedDate

        Dim tmpWindow As New Wangk.ResourceWPF.BackgroundWork(Me) With {
           .Title = "读取数据"
        }

        tmpWindow.Run(Sub(uie)
                          Dim stepCount = 2

                          SalesLocalDatabaseHelper.Clear()

                          uie.Write("连接 Salesforce", 0 * 100 / stepCount)
                          SalesforceHelper.Init()

                          uie.Write("读取 Salesforce 数据", 1 * 100 / stepCount)
                          SalesforceHelper.ReadData(uie)

                          SalesforceHelper.Logout()

                      End Sub)

        If tmpWindow.Error IsNot Nothing Then
            MsgBox(tmpWindow.Error.Message, MsgBoxStyle.Critical, tmpWindow.Title)
            Exit Sub
        End If

        Wangk.ResourceWPF.Toast.ShowSuccess(Me, "读取数据完成")

    End Sub

    Private Sub DeleteData(sender As Object, e As RoutedEventArgs)

        AppSettingHelper.Instance.SalesforceUserName = SalesforceUserName.Text
        AppSettingHelper.Instance.SalesforcePassword = SalesforcePassword.Password
        AppSettingHelper.Instance.DeleteBeforeDate = DeleteBeforeDate.SelectedDate

        Dim tmpWindow As New Wangk.ResourceWPF.BackgroundWork(Me) With {
           .Title = "删除数据"
        }

        tmpWindow.Run(Sub(uie)
                          Dim stepCount = 2

                          uie.Write("连接 Salesforce", 0 * 100 / stepCount)
                          SalesforceHelper.Init()

                          uie.Write("删除 Salesforce 数据", 1 * 100 / stepCount)
                          SalesforceHelper.DeleteTaskData(uie)

                          SalesforceHelper.Logout()

                      End Sub)

        If tmpWindow.Error IsNot Nothing Then
            MsgBox(tmpWindow.Error.Message, MsgBoxStyle.Critical, tmpWindow.Title)
            Exit Sub
        End If

        Wangk.ResourceWPF.Toast.ShowSuccess(Me, "删除数据完成")

    End Sub

End Class
