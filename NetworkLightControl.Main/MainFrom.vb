Imports System.IO
Imports System.Net

Public Class MainFrom

    Private _udpSender As UDPSender
    Private Const PIN_COUNT = 7

    Private Sub MainFrom_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CreateUDPSender(txtDstIP.Text, txtDstPort.Text)
        PopulateComboBoxes()
    End Sub

    Private Sub CreateUDPSender(ByVal ipString As String, ByVal portString As String)
        Dim ip As New IPAddress(New Byte())
        Dim port As Integer = 0

        If IPAddress.TryParse(ipString, ip) AndAlso Integer.TryParse(portString, port) Then
            _udpSender = New UDPSender(ip, port, PIN_COUNT)
        Else
            MessageBox.Show("Please Enter Valid Ip Address and Port Number")
        End If
    End Sub

    Private Sub PopulateComboBoxes()
        Dim cboCount As Integer = 8
        Dim styles() As String = {"Solid", "Blink"}
        Dim speeds() As String = {"Slow", "Medium", "Fast"}

        Dim allStyles As String() = {"Solid", "Cascade", "Random"}
        cboStyleAll.Items.AddRange(allStyles)
        cboStyleAll.SelectedIndex = 0

        cboSpeedAll.Items.AddRange(speeds)
        cboSpeedAll.SelectedIndex = 1

        Dim cboStyle As ComboBox
        Dim cboSpeed As ComboBox

        For i As Integer = 0 To cboCount
            cboStyle = CType(Me.Controls.Find("cboStyle" + i.ToString(), False)(0), ComboBox)
            cboSpeed = CType(Me.Controls.Find("cboSpeed" + i.ToString(), False)(0), ComboBox)

            cboStyle.Items.AddRange(styles)
            cboSpeed.Items.AddRange(speeds)

            cboStyle.SelectedIndex = 0
            cboSpeed.SelectedIndex = 1
        Next
    End Sub


#Region "StatusButtons"

    Public Sub ChangeButtonStatus(ByVal pin As Integer, ByVal button As Button)
        Dim cboStyle As New ComboBox
        Dim cboSpeed As New ComboBox

        cboStyle = CType(Me.Controls.Find("cboStyle" + pin.ToString(), False)(0), ComboBox)
        cboSpeed = CType(Me.Controls.Find("cboSpeed" + pin.ToString(), False)(0), ComboBox)
        Dim mode As String = cboStyle.SelectedItem.ToString
        Dim speed As String = cboSpeed.SelectedItem.ToString
        Select Case mode
            Case "Solid"
                SolidSwitch(pin, button)
            Case "Blink"
                BlinkSwitch(pin, button, speed)
        End Select
    End Sub

    Private Sub SolidSwitch(ByVal pin As Integer, ByVal button As Button)
        If button.Text = "On" Then
            txtLog.Text &= _udpSender.SetPinSolid(pin, 0)
            button.BackColor = Color.Firebrick
            button.Text = "Off"
        Else
            txtLog.Text += _udpSender.SetPinSolid(pin, 1)
            button.BackColor = Color.Green
            button.Text = "On"
        End If
    End Sub

    Private Sub BlinkSwitch(ByVal pin As Integer, ByVal button As Button, ByVal speed As String)
        If button.Text = "On" Then
            txtLog.Text += _udpSender.BlinkPin(pin, 0, ParseSpeed(speed))
            button.BackColor = Color.Firebrick
            button.Text = "Off"
        Else
            txtLog.Text += _udpSender.BlinkPin(pin, 1, ParseSpeed(speed))
            button.BackColor = Color.Green
            button.Text = "On"
        End If
    End Sub

    Private Sub ChangeAllButtonStatus(ByVal button As Button)
        Dim ip As New IPAddress(New Byte())
        Dim port As Integer

        If (System.Net.IPAddress.TryParse(txtDstIP.Text, ip) AndAlso Integer.TryParse(txtDstPort.Text, port)) Then
            Dim styleFilter As String = cboStyleAll.SelectedItem.ToString
            Dim speedFilter As String = cboSpeedAll.SelectedItem.ToString

            If button.Text = "Off" Then
                button.BackColor = Color.Green
                button.Text = "On"

                SetAllPinStatus(True, speedFilter, styleFilter)
                StyleSelectAll(1, speedFilter)
            ElseIf button.Text = "On" Then
                button.BackColor = Color.Firebrick
                button.Text = "Off"

                SetAllPinStatus(False, speedFilter, styleFilter)
                StyleSelectAll(0, cboSpeedAll.SelectedItem.ToString)
            End If
        Else
            MessageBox.Show("Please Enter Valid Ip Address")
        End If
    End Sub

    Private Sub SetAllPinStatus(ByVal setOn As Boolean, ByVal speedValue As String, ByVal styleValue As String)
        Dim backgroundColor As System.Drawing.Color
        Dim buttonText As String

        If setOn Then
            backgroundColor = Color.Green
            buttonText = "On"
        Else
            backgroundColor = Color.Firebrick
            buttonText = "Off"
        End If

        Dim cboStyle As New ComboBox
        Dim cboSpeed As New ComboBox
        Dim btnStatus As New Button

        Dim cboCount As Integer = 7
        For i As Integer = 0 To cboCount
            cboStyle = CType(Me.Controls.Find("cboStyle" + i.ToString(), False)(0), ComboBox)
            cboSpeed = CType(Me.Controls.Find("cboSpeed" + i.ToString(), False)(0), ComboBox)
            btnStatus = CType(Me.Controls.Find("btnStatus" + i.ToString(), False)(0), Button)

            cboStyle.SelectedItem = styleValue
            cboSpeed.SelectedItem = speedValue

            btnStatus.BackColor = backgroundColor
            btnStatus.Text = buttonText
        Next
    End Sub

    Private Sub StyleSelectAll(ByVal onOffvalue As Integer, ByVal speed As String)
        ' Dim pinCount As Integer = 7
        Select Case cboStyleAll.SelectedItem.ToString
            Case "Solid"
                txtLog.Text &= _udpSender.SetAllSolid(onOffvalue)
            Case "Blink"
                'txtLog.Text &= _udpSender.UpdateLightStatusBlinkAll(pinCount, value, speed)
            Case "Cascade"
                txtLog.Text = _udpSender.CascadeAllPins(onOffvalue, ParseSpeed(speed))
            Case "Random"
                txtLog.Text &= _udpSender.BlinkRandomPin(onOffvalue, ParseSpeed(speed))
        End Select
    End Sub

    Private Sub btnStatus0_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatus0.Click
        ChangeButtonStatus(0, btnStatus0)
    End Sub

    Private Sub btnStatus1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatus1.Click
        ChangeButtonStatus(1, btnStatus1)
    End Sub

    Private Sub btnStatus2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatus2.Click
        ChangeButtonStatus(2, btnStatus2)
    End Sub

    Private Sub btnStatus3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatus3.Click
        ChangeButtonStatus(3, btnStatus3)
    End Sub

    Private Sub btnStatus4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatus4.Click
        ChangeButtonStatus(4, btnStatus4)
    End Sub

    Private Sub btnStatus5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatus5.Click
        ChangeButtonStatus(5, btnStatus5)
    End Sub

    Private Sub btnStatus6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatus6.Click
        ChangeButtonStatus(6, btnStatus6)
    End Sub

    Private Sub btnStatus7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatus7.Click
        ChangeButtonStatus(7, btnStatus7)
    End Sub

    Private Sub btnStatusAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatusAll.Click
        ChangeAllButtonStatus(btnStatusAll)
    End Sub

#End Region

    Private Sub btnUpdatePortIP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdatePortIP.Click
        SetAllPinStatus(False, "Medium", "Solid")
        CreateUDPSender(txtDstIP.Text, txtDstPort.Text)
    End Sub

    Private Function ParseSpeed(ByVal speed As String) As Integer
        Dim ticks As Integer
        Select Case speed
            Case "Slow"
                ticks = 2000
            Case "Medium"
                ticks = 1000
            Case "Fast"
                ticks = 500
        End Select

        Return ticks
    End Function

End Class
'
