Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text

Public Class UDPSender

    Private _isActive As Boolean
    Private ReadOnly _pinCount As Integer
    Private ReadOnly _endPoint As IPEndPoint
    Private ReadOnly _udpSocket As Socket

    Public Sub New(ByVal ip As IPAddress, ByVal port As Integer, ByVal pinCount As Integer)
        _pinCount = pinCount
        _endPoint = New IPEndPoint(ip, port)
        'Using the Socket here instead of a UDPClient becasue Socket is thread safe for instance memebers 
        _udpSocket = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
    End Sub

    Private Sub SendUDPData(ByVal sendBytes As Byte())
        Try
            _udpSocket.SendTo(sendBytes, _endPoint)
        Catch ex As Exception
            'Log error
        End Try
    End Sub

    Public Function SetPinSolid(ByVal pin As Integer, ByVal value As Integer) As String
        Dim udpClient As New UdpClient
        Dim sendBytes As Byte() = Encoding.ASCII.GetBytes(pin.ToString & value.ToString)
        SendUDPData(sendBytes)

        Return "Pin: " & pin.ToString() & " Value: " & value.ToString() & vbCrLf
    End Function

    Public Function SetAllSolid(ByVal onOffvalue As Integer) As String
        Dim sendAllSolidThread = New Thread(AddressOf SendAllSolid)
        sendAllSolidThread.Start(onOffvalue)

        Return "Set all pins = " & onOffvalue & vbCrLf
    End Function

    Private Sub SendAllSolid(ByVal onOffValue As Integer)
        Dim sendBytes As Byte()
        For pin As Integer = 0 To _pinCount
            sendBytes = Encoding.ASCII.GetBytes(pin.ToString & onOffValue.ToString)
            SendUDPData(sendBytes)
        Next
    End Sub

    Public Function BlinkPin(ByVal pin As Integer, ByVal onOffvalue As Integer, ByVal speed As Integer) As String
        Dim cd As New ControlData
        cd.SendBytesOn = Encoding.ASCII.GetBytes(pin.ToString & 1)
        cd.SendBytesOff = Encoding.ASCII.GetBytes(pin.ToString & 0)
        cd.Speed = speed
        SetActive(onOffvalue)

        Dim sendBlinkThread = New Thread(AddressOf SendBlink)
        sendBlinkThread.Start(cd)

        Return "Blink Pin: " & pin.ToString() + " Value: " & onOffvalue.ToString() & vbCrLf
    End Function

    Private Sub SendBlink(ByVal cd As ControlData)
        Do While _isActive
            SendUDPData(cd.SendBytesOn)
            Thread.Sleep(cd.Speed)
            SendUDPData(cd.SendBytesOff)
            Thread.Sleep(cd.Speed)
        Loop
    End Sub

    Public Function BlinkRandomPin(ByVal onOffValue As Integer, ByVal speed As Integer) As String
        SetActive(onOffValue)
        Dim sendRandomBlinkThread = New Thread(AddressOf SendBlinkRandom)
        sendRandomBlinkThread.Start(speed)

        Return "Blink Random" & vbCrLf
    End Function

    Private Sub SendBlinkRandom(ByVal speed As Integer)
        Dim sendBytes As Byte()
        Dim pin As Integer = 0
        Dim randomClass As New Random()

        Do While _isActive
            pin = randomClass.Next(8)
            sendBytes = Encoding.ASCII.GetBytes(pin.ToString & "1")
            SendUDPData(sendBytes)

            Thread.Sleep(speed)

            sendBytes = Encoding.ASCII.GetBytes(pin.ToString & "0")
            SendUDPData(sendBytes)
        Loop
    End Sub

    Public Function CascadeAllPins(ByVal onOffValue As Integer, ByVal speed As Integer) As String
        SetActive(onOffValue)
        Dim sendCascadeAllThread = New Thread(AddressOf SendCascadeAll)
        sendCascadeAllThread.Start(speed)

        Return "Cascade All" & vbCrLf
    End Function

    Private Sub SendCascadeAll(ByVal speed As Integer)
        Dim sendBytes As Byte()
        Dim isReverse As Boolean

        Dim pin As Integer = 0
        Do While _isActive
            sendBytes = Encoding.ASCII.GetBytes(pin.ToString & "1")
            SendUDPData(sendBytes)
            Thread.Sleep(speed)

            sendBytes = Encoding.ASCII.GetBytes(pin.ToString & "0")
            SendUDPData(sendBytes)

            If isReverse Then
                pin = pin - 1
            Else
                pin = pin + 1
            End If

            If pin = _pinCount Then
                isReverse = True
            ElseIf pin = 0 Then
                isReverse = False
            End If
        Loop
    End Sub

    Private Sub SetActive(ByVal onOffValue As Integer)
        If onOffValue = 1 Then
            _isActive = True
        Else
            _isActive = False
        End If
    End Sub

End Class
