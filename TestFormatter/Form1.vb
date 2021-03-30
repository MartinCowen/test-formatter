Imports System.Net

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateLangs()
    End Sub
    Private Sub PopulateLangs()
        With cmbFrom
            .Enabled = False
            .Items.Add("MSTest")
            .Enabled = True
        End With

        With cmbTo
            .Enabled = False
            .Items.Add("minunit")
            .Enabled = True
        End With

        cmbFrom.SelectedIndex = 0
        cmbTo.SelectedIndex = 0

    End Sub


    Private Sub txtInput_TextChanged(sender As Object, e As EventArgs) Handles txtInput.TextChanged
        StartSourceFormat()
    End Sub

    Private Sub cmbLangs_TextChanged(sender As Object, e As EventArgs) Handles cmbFrom.TextChanged
        StartSourceFormat()
    End Sub

    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        My.Computer.Clipboard.SetText(txtOutput.Text)
    End Sub
    Private Sub StartSourceFormat()
        Dim nl As String = System.Environment.NewLine
        txtOutput.Text = ""
        For Each ln As String In txtInput.Text.Split(nl)
            txtOutput.Text &= LineFormatMSTestToMinunit(ln)
        Next ln
    End Sub
    Private Function LineFormatMSTestToMinunit(ln As String) As String
        Dim nl As String = System.Environment.NewLine
        Dim r As String = ""
        Static test_name As String = "" 'test name needs to be retained between lines

        If ln.Contains("TEST_METHOD") Then
            test_name = Trim(ln.Split("(")(1).Split(")")(0))
            r &= "static char* mu_test" & test_name & "(void)" & nl
        ElseIf ln.Contains("Assert::AreEqual") Then
            Dim s As String
            'remove the uint32_t casts
            s = ln.Replace("(uint32_t)", "")
            'replace the comma with equal test
            s = s.Replace(",", " ==")
            'create the new assert
            s = s.Replace("Assert::AreEqual(", "mu_assert(""" & test_name & " failed" & """, ")
            r &= s & nl
        Else
            r &= ln & nl
        End If
        Return r
    End Function


    ''' <summary>
    ''' Counts instances of substrings within strings
    ''' </summary>
    ''' <param name="s">string to be searched</param>
    ''' <param name="substr">substring</param>
    ''' <returns>Number of instances</returns>
    Public Function CountInstancesOfSubstrings(s As String, substr As String) As Integer
        Dim c As Integer = 0
        For i As Integer = 0 To s.Length - substr.Length
            If s.Substring(i, substr.Length) = substr Then
                c += 1
            End If
        Next i
        Return c
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtInput.Text = String.Empty
        txtInput.Focus()
    End Sub

End Class
