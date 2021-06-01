Imports Microsoft.Win32

Module Module1
    Const AppName As String = "HideUser"

    Sub Main()

        Dim mode As String = Nothing
        Dim username As String = Nothing

        'Get command line arguments
        Dim s() As String = Environment.GetCommandLineArgs()
        For i As Integer = 1 To s.Length - 1
            Select Case LCase(s(i))
                Case "--list"
                    mode = "list"
                    Exit For
                Case "--hide"
                    mode = "hide"
                    username = s(i + 1)
                    Exit For
                Case "--show"
                    mode = "show"
                    username = s(i + 1)
                    Exit For
                Case Else
                    ShowHelp()
                    Exit For
            End Select
        Next

        If mode = "" Then
            ShowHelp()
        ElseIf mode <> "list" And username = "" Then
            ShowHelp()
        End If

        Console.WriteLine("")

        Try
            Dim SpecialAccountsPath As String = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts"
            Dim UserListPath As String = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList"

            Dim sa As RegistryKey = Nothing
            Try
                sa = Registry.LocalMachine.OpenSubKey(SpecialAccountsPath, True)
            Catch ex As Exception
                Registry.LocalMachine.CreateSubKey(SpecialAccountsPath)
                sa = Registry.LocalMachine.OpenSubKey(SpecialAccountsPath, True)
            End Try

            Dim ul As RegistryKey = Nothing
            Try
                ul = Registry.LocalMachine.OpenSubKey(UserListPath, True)
            Catch ex As Exception
                Registry.LocalMachine.CreateSubKey(UserListPath)
                ul = Registry.LocalMachine.OpenSubKey(UserListPath, True)
            End Try

            If mode = "list" Then
                Console.WriteLine("Users hidden on logon screen:")
                Console.WriteLine("")
                Try
                    For Each user As String In ul.GetValueNames()
                        Console.WriteLine(user)
                    Next
                Catch ex As Exception
                    Console.WriteLine("No users hidden")
                End Try
            ElseIf mode = "hide" Then
                ul.SetValue(username, 0, RegistryValueKind.DWord)
                Console.WriteLine(String.Format("Added ""{0}"" to users hidden on logon screen", username))
            ElseIf mode = "show" Then
                If ul.GetValueNames.Contains(username) Then
                    ul.DeleteValue(username)
                    Console.WriteLine(String.Format("Removed ""{0}"" from users hidden on logon screen", username))
                Else
                    Console.WriteLine(String.Format("User ""{0}"" is not hidden on logon screen", username))
                End If
            End If
        Catch ex As Exception
            Console.WriteLine("Error. Are you running as Administrator?")
        End Try

        Console.WriteLine("")

    End Sub

    Sub ShowHelp()
        Console.WriteLine("")
        Console.Write("Shows/hides user accounts from Windows logon screen.

Usage: " & AppName & " [options] <Username>

Options:
    --list      Lists all users hidden from logon screen. Username argument not required here.
    --hide      Hides user from logon screen.
    --show      Shows user on logon screen.

    --help      Shows this help
")
        Console.WriteLine("")
        End
    End Sub

End Module