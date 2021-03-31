Imports System.Net


Public Class Form1
    Private Delegate Function linefunction_t(ln As String) As String 'function pointer type, see http://www.vb-helper.com/howto_2005_delegate_variable.html

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateLangs()
    End Sub
    Private Sub PopulateLangs()
        With cmbFrom
            .Enabled = False
            .Items.Add("MSTest")
            .Items.Add("minunit")
            .Enabled = True
        End With

        With cmbTo
            .Enabled = False
            .Items.Add("MSTest")
            .Items.Add("minunit")
            .Enabled = True
        End With

        cmbFrom.SelectedIndex = 0
        cmbTo.SelectedIndex = 1

    End Sub


    Private Sub txtInput_TextChanged(sender As Object, e As EventArgs) Handles txtInput.TextChanged
        StartSourceFormat()
    End Sub



    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        My.Computer.Clipboard.SetText(txtOutput.Text)
    End Sub
    Private Sub StartSourceFormat()
        'check that conversion is required
        If cmbFrom.SelectedIndex < 0 OrElse cmbTo.SelectedIndex < 0 OrElse cmbFrom.SelectedIndex = cmbTo.SelectedIndex Then Exit Sub

        'select the conversion function
        Dim linefunction As linefunction_t
        If cmbFrom.SelectedItem.ToString = "MSTest" AndAlso cmbTo.SelectedItem.ToString = "minunit" Then
            linefunction = AddressOf LineFormatMSTestToMinunit
        ElseIf cmbFrom.SelectedItem.ToString = "minunit" AndAlso cmbTo.SelectedItem.ToString = "MSTest" Then
            linefunction = AddressOf LineFormatMinunitToMSTest
        Else
            linefunction = Nothing
        End If

        Dim nl As String = System.Environment.NewLine
        txtOutput.Text = ""

        'convert each line
        For Each ln As String In txtInput.Text.Split(nl)
            If linefunction IsNot Nothing Then txtOutput.Text &= linefunction(ln)
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
        ElseIf ln.Contains("Assert::AreNotEqual") Then
            Dim s As String
            'remove the uint32_t casts
            s = ln.Replace("(uint32_t)", "")
            'replace the comma with not equal test
            s = s.Replace(",", " !=")
            'create the new assert
            s = s.Replace("Assert::AreNotEqual(", "mu_assert(""" & test_name & " failed" & """, ")
            r &= s & nl
        Else
            r &= ln & nl
        End If
        Return r
    End Function

    Private Function LineFormatMinunitToMSTest(ln As String) As String
        Dim nl As String = System.Environment.NewLine
        Dim r As String = ""
        Static test_name As String = "" 'test name needs to be retained between lines

        Dim bFoundRelation As Boolean
        Dim bFoundEq As Boolean
        Dim bFoundNeq As Boolean
        Dim firstoperand As String = ""
        Dim secondoperand As String = ""

        If ln.Contains("static char*") OrElse ln.Contains("static char *") Then
            Dim fn As String = ln.Split(" ")(2)
            test_name = fn.Replace("mu_test", "").Replace("(void)", "")
            r &= "TEST_METHOD(" & test_name & ")" & nl
        ElseIf ln.Contains("mu_assert") Then
            Dim p() As String = ln.Split("""") 'split on " not space or comma because must have quote, and comma could be inside a string

            If p.Length >= 2 Then
                Dim q As String = p(2).Replace(",", "") '" exp == fun..
                Dim s() As String = q.Split(" ")
                For Each s1 As String In s
                    If s1.Contains("==") Then
                        bFoundRelation = True
                        bFoundEq = True
                    ElseIf s1.Contains("!=") Then
                        bFoundRelation = True
                        bFoundNeq = True
                    ElseIf firstoperand = "" Then
                        firstoperand = s1
                    ElseIf secondoperand = "" Then
                        secondoperand = s1
                    End If
                Next s1
                If bFoundRelation Then
                    If bFoundEq Then
                        r = vbTab & "Assert::AreEqual("
                    End If
                    If bFoundNeq Then
                        r = vbTab & "Assert::AreNotEqual("
                    End If
                    r &= "(uint32_t)" & firstoperand & ", (uint32_t)" & secondoperand & nl
                End If
            End If
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

    Private Sub cmbFrom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFrom.SelectedIndexChanged
        StartSourceFormat()
    End Sub

    Private Sub cmbTo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTo.SelectedIndexChanged
        StartSourceFormat()
    End Sub
End Class
