
Imports Microsoft.Win32
Imports System.Xml.Linq


Class Window1

    ''' <summary>
    ''' Ricerca per titolo
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Query_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnQuery.Click

        Dim searchTitle As String = tbTitle.Text
        Dim searchAuthor As String = tbAuthor.Text

        If String.IsNullOrEmpty(searchTitle) And String.IsNullOrEmpty(searchAuthor) Then
            MsgBox("Nessun testo di ricerca definito", MsgBoxStyle.Critical)
            Return
        End If

        Dim books = Nothing

        Try
            If String.IsNullOrEmpty(searchAuthor) Then
                ' Create the query per titolo
                books = From c In XElement.Load(btnFile.Content.ToString).Elements("record")
                        Where c.Element("title").ToString.ToUpper.Contains(searchTitle.ToUpper) 'Select c.Title, c.authors
            ElseIf String.IsNullOrEmpty(searchTitle) Then
                ' Create the query per titolo
                books = From c In XElement.Load(btnFile.Content.ToString).Elements("record")
                        Where c.Element("authors").ToString.ToUpper.Contains(searchAuthor.ToUpper) 'Select c.Title, c.authors
            Else

            End If
        Catch ex As Exception
            MsgBox("Errore grave: " & ex.Message, MsgBoxStyle.Critical)
            Return
        End Try

        ListBox1.Items.Clear()

        Dim text As String
        Dim addAuthor As Boolean = CheckBoxAuthors.IsChecked
        Dim addFormats As Boolean = CheckBoxFormats.IsChecked
        Dim addTags As Boolean = CheckBoxTags.IsChecked
        Dim count As Integer = 0

        ' Execute the query
        If books IsNot Nothing Then
            For Each book In books
                count += 1
                text = count.ToString + " - " + book.Element("title").Value
                If (addAuthor) Then text += " - " + book.Element("authors").Value
                If (addFormats) Then text += " - " + book.Element("_formats").Value
                If (addTags) Then text += " - " + book.Element("tags").Value
                ListBox1.Items.Add(text)
            Next
        Else
            ListBox1.Items.Add("Problema creazione oggetto books")
        End If

        'Pause the application 
        ListBox1.Items.Add(String.Format("-> {0} risultati", count))
    End Sub


    ''' <summary>
    ''' Selezione del file in cui ricercare
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFile.Click

        Dim dlg As New OpenFileDialog
        dlg.Filter = "XML files (*.xml)|*.xml|Tutti i files (*.*)|*.*"
        If True = dlg.ShowDialog Then
            btnFile.Content = dlg.FileName
            My.Settings.Datafile = dlg.FileName
        End If

    End Sub


    ''' <summary>
    ''' Inizialmente setta i controlli con le impostazioni salvate l'ultima volta
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        If Not String.IsNullOrEmpty(My.Settings.Datafile) Then btnFile.Content = My.Settings.Datafile
        CheckBoxAuthors.IsChecked = My.Settings.ShowAuthors
        CheckBoxFormats.IsChecked = My.Settings.ShowFormats
        CheckBoxTags.IsChecked = My.Settings.ShowTags

        Dim memSize As System.Drawing.Size = My.Settings.WindowSize
        If (memSize.Width <> 0) Then
            Me.Width = memSize.Width
            Me.Height = memSize.Height
        End If

        Title = Title + " - v." + System.Windows.Application.ResourceAssembly.ImageRuntimeVersion

    End Sub



    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        '
        My.Settings.ShowAuthors = CheckBoxAuthors.IsChecked
        My.Settings.ShowFormats = CheckBoxFormats.IsChecked
        My.Settings.ShowTags = CheckBoxTags.IsChecked
        '
        My.Settings.WindowSize = New System.Drawing.Size(Me.Width, Me.Height)

        My.Settings.Save()
    End Sub
End Class
