
Imports System.IO
Imports System.IO.File
Imports System.Threading
Imports System.Console

Module Module1

    Public Class Snake
        Public X As Queue(Of Integer) = New Queue(Of Integer)() 'X coord Body
        Public Y As Queue(Of Integer) = New Queue(Of Integer)() 'Y coord Body
        Public H(1) As Integer 'Position of Head
        Public L As Integer 'Length
        Public S As Integer 'Speed
        Public M As Integer 'Point Modifier
        Public D As String 'Direction

    End Class

    Public Class Grid
        Public R As Random = New Random 'Random num gen
        Public G(20, 20) As String 'Grid
        Public F() As Integer = {R.Next(2, 19), R.Next(2, 19)} 'Location of Fruit
    End Class

    Public Class Scores
        Public N(2) As Char 'Three letter name
        Public S As String 'Score
    End Class

    Public Class FileData
        Public Len As Integer = FileLength() 'Gets the amount of rows there are in file
        Public N(Len) As String 'Stores all the Players
        Public S(Len) As Integer 'Stores all the Scores

    End Class

    Sub Main()
        Dim F As New FileData
        MainMenu(F)

    End Sub

    Sub MainMenu(ByRef F As FileData)
        Clear()
        Dim Temp As String
        WriteLine("Would you like to:{0}1. Play{0}2. View LeaderBoard{0}3. Quit", vbCrLf)
        Temp = ReadLine()
        If Temp = "1" Then
            Play(F)

        ElseIf Temp = "2" Then
            LeaderBoard(F)

        ElseIf Temp = "3" Then
            Environment.Exit(0)

        Else
            WriteLine("Please try again")
            MainMenu(F)

        End If
    End Sub


    Sub GetScore(Sc As Scores, ByRef F As FileData)
        Dim temp As String = ""
        Dim i As Integer = 0 'index
        Dim lop As Boolean = True
        Sc.N = {"A", "A", "A"}

        While lop = True
            Thread.Sleep(67)
            Clear()

            If KeyAvailable Then
                temp = ReadKey(True).KeyChar

            Else
                temp = ""

            End If

            If temp = "w" Then
                Sc.N(i) = swapLetters(Sc.N(i), 1)

            ElseIf temp = "s" Then
                Sc.N(i) = swapLetters(Sc.N(i), -1)

            ElseIf temp = "a" Then
                If i > 0 Then
                    i -= 1

                Else
                    i = 2

                End If

            ElseIf temp = "d" Then

                If i < 2 Then
                    i += 1

                Else
                    i = 0

                End If

            ElseIf temp = "q" Then
                SaveScores(Sc)
                lop = False

            End If
            WriteLine(Sc.N(0) & Sc.N(1) & Sc.N(2) & " score: " & Sc.S & vbCrLf & StrDup(i, " ") & "^" & vbCrLf & "Press q to continue")

        End While
    End Sub

    Function FileLength() As Integer
        Dim LineCount As Integer = (ReadAllLines("Snake Scores.txt").Length) - 1

        Return LineCount
    End Function

    Sub LeaderBoard(ByRef File As FileData)
        Clear()

        WriteLine("Name ║ Score")
        Dim Count As Integer = 0
        Dim MyReader As New FileIO.TextFieldParser("Snake Scores.txt")
        Dim currentRow As String()
        Dim rows As Integer = 0
        Dim currentField As String

        File.Len = FileLength()
        ReDim File.N(File.Len)
        ReDim File.S(File.Len)

        MyReader.TextFieldType = FileIO.FieldType.Delimited
        MyReader.SetDelimiters(",")

        While Not MyReader.EndOfData
            currentRow = MyReader.ReadFields()

            For Each currentField In currentRow
                If Count = 0 Then
                    File.N(rows) = currentField
                    Count += 1

                ElseIf Count = 1 Then
                    File.S(rows) = currentField
                    Count = 0

                End If

            Next
            rows += 1

        End While

        Dim newn As Integer
        Dim tempI As Integer
        Dim tempS As String

        Dim n As Integer = File.Len

        Do
            newn = 0
            For i As Integer = 1 To n

                If File.S(i - 1) > File.S(i) Then
                    tempI = File.S(i - 1)
                    tempS = File.N(i - 1)
                    File.S(i - 1) = File.S(i)
                    File.N(i - 1) = File.N(i)
                    File.S(i) = tempI
                    File.N(i) = tempS
                    Count += 1
                    newn = i

                End If

            Next
            n = newn

        Loop Until n = 0

        Dim base As Integer

        If File.Len - 9 >= 0 Then
            base = File.Len - 9

        Else
            base = 0

        End If

        For i As Integer = File.Len To base Step -1
            WriteLine(File.N(i) & StrDup(3 - File.N(i).Length(), " ") & "  ║ " & File.S(i))

        Next
        Console.WriteLine("Of {0} players", File.Len)
        MyReader.Close()

        WriteLine(vbCrLf & "Press enter to return to main menu")
        ReadLine()
        MainMenu(File)
    End Sub

    Function swapLetters(currentLetter As Char, directionMod As Integer) As Char

        If Asc(currentLetter) + directionMod < 65 Then
            Return "Z"

        ElseIf Asc(currentLetter) + directionMod > 90 Then
            Return "A"

        Else
            Return Chr(Asc(currentLetter) + directionMod)

        End If
    End Function

    Sub SaveScores(Sc As Scores)
        Dim F As StreamWriter
        Dim Packet As String = Sc.N(0) + Sc.N(1) + Sc.N(2) + "," + Sc.S

        F = My.Computer.FileSystem.OpenTextFileWriter("Snake Scores.txt", True)
        F.WriteLine(Packet)
        F.Close()

    End Sub

    Sub Play(ByRef F As FileData)
        Dim fin As Boolean
        Dim GameLoop As Boolean = True
        Dim S As New Snake
        Dim G As New Grid
        Dim Sc As New Scores
        While fin = False

            S.X.Clear()
            S.Y.Clear()
            S.H(0) = G.R.Next(2, 19)
            S.H(1) = G.R.Next(2, 19)
            S.L = 1
            S.X.Enqueue(S.H(0))
            S.Y.Enqueue(S.H(1))
            S.D = Nothing
            S.S = SpeedSelect(S)

            While GameLoop = True
                Thread.Sleep(S.S)
                Clear()
                core(GameLoop, S, G, Sc)
            End While

            fin = GameOver(GameLoop, Sc, F)

        End While
        MainMenu(F)
    End Sub

    Function SpeedSelect(ByRef S As Snake) As Integer
        Dim temp As String
        Clear()
        WriteLine("What game difficult do you want {0}1: Easy{0}2: Normal{0}3: Hard{0}4: Insane", vbCrLf)
        temp = ReadLine()

        If temp = "1" Then
            S.M = 50
            Return 100

        ElseIf temp = "2" Then
            S.M = 100
            Return 85

        ElseIf temp = "3" Then
            S.M = 200
            Return 65

        ElseIf temp = "4" Then
            S.M = 400
            Return 50
        Else
            WriteLine("Please, try again")
            Return SpeedSelect(S)

        End If

    End Function

    Function GameOver(ByRef GameLoop As Boolean, Sc As Scores, ByRef F As FileData) As Boolean
        Dim temp As String

        WriteLine("Press a key To Continue")
        ReadLine()
        Clear()
        GetScore(Sc, F)

        Clear()
        WriteLine("Would you Like To play again? (y/n)")
        temp = ReadLine()

        If temp.ToLower = "y" Then
            GameLoop = True
            Return False

        ElseIf temp.ToLower = "n" Then
            Return True

        Else
            WriteLine("Try agian please")
            Return GameOver(GameLoop, Sc, F)

        End If
    End Function

    Sub core(ByRef GameLoop As Boolean, ByRef S As Snake, ByRef G As Grid, ByRef Sc As Scores)

        mazeSetup(S, G)
        mazeDisplay(S, G, Sc)
        playerMove(S)

        If G.G(S.H(0), S.H(1)) = "█" Or G.G(S.H(0), S.H(1)) = "o" Then
            GameLoop = False
            WriteLine("{0}You've lost{0}", vbCrLf)

        ElseIf S.H(0) = G.F(0) And S.H(1) = G.F(1) Then
            G.F = {G.R.Next(2, 19), G.R.Next(2, 19)}
            S.L += 1

        Else
            S.X.Dequeue()
            S.Y.Dequeue()

        End If
        WriteLine(" ")

    End Sub

    Sub mazeSetup(S As Snake, ByRef G As Grid)

        For i = 1 To 20
            For j = 1 To 20
                G.G(i, j) = " "

            Next

        Next

        For i = 1 To 20
            For j = 1 To 20
                If i = G.F(0) And j = G.F(1) Then
                    G.G(i, j) = "E"

                ElseIf i = 1 Or i = 20 Or j = 1 Or j = 20 Then
                    G.G(i, j) = "█"

                End If

                For k As Integer = 0 To S.L
                    If i = S.X(k) And j = S.Y(k) Then
                        G.G(i, j) = "o"

                    End If

                Next

                If i = S.H(0) And j = S.H(1) Then
                    G.G(i, j) = "O"

                End If

            Next

        Next

    End Sub

    Sub mazeDisplay(S As Snake, G As Grid, ByRef Sc As Scores)
        Dim sb As New Text.StringBuilder


        For i As Integer = 1 To 20
            For j As Integer = 1 To 20
                sb.Append(G.G(i, j))
            Next
            sb.Append(vbCrLf)
        Next

        Write(sb)

        Sc.S = S.L * S.M
        WriteLine("Score: " & Sc.S)

    End Sub


    Sub playerMove(ByRef S As Snake)

        If KeyAvailable Then
            S.D = ReadKey(True).KeyChar

        End If

        If S.D = "w" Then
            S.H(0) -= 1

        ElseIf S.D = "s" Then
            S.H(0) += 1

        ElseIf S.D = "a" Then
            S.H(1) -= 1

        ElseIf S.D = "d" Then
            S.H(1) += 1

        End If

        S.X.Enqueue(S.H(0))
        S.Y.Enqueue(S.H(1))

    End Sub

End Module

