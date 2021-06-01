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
                    If s.Length - 1 < i + 1 Then
                        ShowHelp()
                    End If
                    mode = "hide"
                    username = s(i + 1)
                    Exit For
                Case "--show"
                    If s.Length - 1 < i + 1 Then
                        ShowHelp()
                    End If
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
            Dim WinLogonPath As String = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
            Dim SpecialAccountsPath As String = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts"
            Dim UserListPath As String = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList"

            Dim sa As RegistryKey = Registry.LocalMachine.OpenSubKey(SpecialAccountsPath, True)
            If sa Is Nothing Then
                Dim wlp = Registry.LocalMachine.OpenSubKey(WinLogonPath, True)
                wlp.CreateSubKey("SpecialAccounts")
                sa = Registry.LocalMachine.OpenSubKey(SpecialAccountsPath, True)
            End If

            Dim ul As RegistryKey = Registry.LocalMachine.OpenSubKey(UserListPath, True)
            If ul Is Nothing Then
                sa.CreateSubKey("UserList")
                ul = Registry.LocalMachine.OpenSubKey(UserListPath, True)
            End If

            If mode = "list" Then
                Console.WriteLine("Users hidden on logon screen:")
                Console.WriteLine("")
                Dim UserListValues() As String = ul.GetValueNames()
                If UserListValues.Count = 0 Then
                    Console.WriteLine("No users hidden")
                Else
                    For Each user As String In ul.GetValueNames()
                        Console.WriteLine(user)
                    Next
                End If
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
            Dim st As New StackTrace(True)
            st = New StackTrace(ex, True)
            Console.WriteLine("Line: " & st.GetFrame(0).GetFileLineNumber().ToString, "Error")

            Console.WriteLine("Error. Are you running as Administrator?")
        End Try

        Console.WriteLine("")

    End Sub

    Sub ShowHelp()
        Console.WriteLine("")
        Console.Write("Hide/show users on Windows logon screen.

Usage: " & AppName & " [options] <Username>

USERNAME IS CASE SENSITIVE

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