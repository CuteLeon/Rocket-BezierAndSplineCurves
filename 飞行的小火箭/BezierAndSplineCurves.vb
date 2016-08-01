Imports System.Drawing.Drawing2D

Public Class BezierAndSplineCurves
    '创建路径使用的坐标个数
    Private Const PointBound As Integer = 10
    Private Const LastPointsCount As Integer = 4
    '绘图显示的数据(依次为：路径曲线、关键坐标、路径坐标)
    Dim ShowPath As Boolean = False
    Dim ShowCurvedPoints As Boolean = False
    Dim ShowPathPoints As Boolean = False

    Dim CurvedPoints(PointBound) As PointF      '关键坐标
    Dim PathPoints As PointF()                            '路径坐标
    Dim PathPointIndex As Integer = 0               '当前路径坐标标识
    Dim LastPoint As PointF = New PointF(0, 0)  '前一坐标(用于计算角度)
    Dim NextPoint As PointF                               '后一坐标(用于计算角度)
    Dim Angle As Single                                      '火箭旋转角度
    Dim RocketBitmap As Bitmap                        '火箭位图

#Region "窗体"

    Private Sub BezierAndSplineCurves_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NewPathAndPoints()
        RocketEngine.Start()
    End Sub

    Private Sub RocketEngine_Tick(sender As Object, e As EventArgs) Handles RocketEngine.Tick
        LastPoint = PathPoints(PathPointIndex)
        PathPointIndex += 1
        If PathPointIndex = UBound(PathPoints) Then NewPathAndPoints()

        NextPoint = PathPoints(PathPointIndex + 1)
        Angle = Math.Atan2(LastPoint.Y - NextPoint.Y, NextPoint.X - LastPoint.X)
        Angle = -Angle / (2 * Math.PI) * 360
        RocketBitmap = GetRotateBitmap(My.Resources.RocketResource.Rocket, Angle)
        TheRocket.Image = RocketBitmap

        TheRocket.Location = New Point(PathPoints(PathPointIndex).X - 32, PathPoints(PathPointIndex).Y - 32)

        Label2.Text = PathPoints(PathPointIndex).ToString & vbCrLf & Angle.ToString("###") & vbCrLf & Math.Sqrt((LastPoint.X - PathPoints(PathPointIndex).X) ^ 2 + (LastPoint.Y - PathPoints(PathPointIndex).Y) ^ 2).ToString("###") & vbCrLf & PathPointIndex & " / " & PathPoints.Count

        DrawImage()

        '每次处理完毕强制回收内存
        GC.Collect()
    End Sub

#End Region

#Region "功能函数"
    Private Sub NewPathAndPoints()
        Dim Index As Integer
        For Index = 1 To LastPointsCount
            CurvedPoints(Index - 1) = CurvedPoints(PointBound - LastPointsCount + Index)
        Next
        For Index = LastPointsCount To PointBound
            CurvedPoints(Index) = New PointF(VBMath.Rnd * Me.Width, VBMath.Rnd * Me.Height)
        Next

        PathPoints = GetCurvedPoints(CurvedPoints, PointBound - 3)
        PathPointIndex = 0
    End Sub

    Private Function GetCurvedPoints(ByVal DrawPoints As PointF(), ByVal NumberOfSegments As Integer) As PointF()
        Dim MyGraphicsPath As GraphicsPath = New GraphicsPath
        Dim MyPoints As PointF()
        MyGraphicsPath.AddCurve(DrawPoints, 2, NumberOfSegments, 0.5)
        MyGraphicsPath.Flatten()
        MyPoints = MyGraphicsPath.PathPoints
        MyGraphicsPath.Dispose()
        Return MyPoints
    End Function

    Private Function GetRotateBitmap(ByVal BitmapRes As Bitmap, ByVal Angle As Single) As Bitmap
        Dim ReturnBitmap As New Bitmap(BitmapRes.Width, BitmapRes.Height)
        Dim MyGraphics As Graphics = Graphics.FromImage(ReturnBitmap)
        MyGraphics.SmoothingMode = SmoothingMode.HighQuality
        MyGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality
        MyGraphics.TranslateTransform(BitmapRes.Width / 2, BitmapRes.Height / 2)
        MyGraphics.RotateTransform(Angle, MatrixOrder.Prepend)
        MyGraphics.TranslateTransform(-BitmapRes.Width / 2, -BitmapRes.Height / 2)
        MyGraphics.DrawImage(BitmapRes, 0, 0, BitmapRes.Width, BitmapRes.Height)
        MyGraphics.Dispose()
        Return ReturnBitmap
    End Function

    Private Sub DrawImage()
        If Not (ShowCurvedPoints Or ShowPath Or ShowPathPoints) Then Me.BackgroundImage = Nothing : Exit Sub

        Dim MyBitmap As Bitmap = New Bitmap(Me.Width, Me.Height)
        Dim MyGraphics As Graphics = Graphics.FromImage(MyBitmap)
        MyGraphics.SmoothingMode = SmoothingMode.HighQuality
        MyGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality

        If ShowPath Then MyGraphics.DrawCurve(New Pen(Color.Red, 2), CurvedPoints)

        If ShowPathPoints Then
            For Each a As PointF In PathPoints
                MyGraphics.FillEllipse(Brushes.Pink, New RectangleF(a.X, a.Y, 5, 5))
            Next
        End If

        If ShowCurvedPoints Then
            For Index As Integer = LBound(CurvedPoints) To UBound(CurvedPoints)
                MyGraphics.DrawEllipse(New Pen(Color.Yellow, 3), New Rectangle(CurvedPoints(Index).X - 2, CurvedPoints(Index).Y - 2, 4, 4))
                MyGraphics.DrawString(Index.ToString, Me.Font, Brushes.Yellow, CurvedPoints(Index).X + 2, CurvedPoints(Index).Y + 2)
            Next
        End If

        Me.BackgroundImage = MyBitmap
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        ShowPath = CheckBox1.Checked
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        ShowCurvedPoints = CheckBox2.Checked
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        ShowPathPoints = CheckBox3.Checked
    End Sub

#End Region

End Class