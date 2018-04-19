
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

        If String.IsNullOrEmpty(search) And String.IsNullOrEmpty(searchAuthor) Then
            MsgBox("Nessun testo di ricerca definito", MsgBoxStyle.Critical)
            Return
        End If

        Dim books

        Try
            ' Create the query
            books = From c In XElement.Load(btnFile.Content.ToString).Elements("record") _
                        Where c.Element("title").ToString.ToUpper.Contains(search.ToUpper) 'Select c.Title, c.authors

        Catch ex As Exception
            MsgBox("Errore grave: " & ex.Message, MsgBoxStyle.Critical)
            Return
        End Try

        ListBox1.Items.Clear()

        Dim text As String
        Dim addAuthor As Boolean = CheckBoxAuthors.IsChecked
        Dim addFormats As Boolean = CheckBoxFormats.IsChecked
        Dim count As Integer = 0

        ' Execute the query
        For Each book In books
            text = book.Element("title").Value
            If (addAuthor) Then text += " - " + book.Element("authors").Value
            If (addFormats) Then text += " - " + book.Element("_formats").Value
            ListBox1.Items.Add(text)
            count += 1
        Next

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
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        My.Settings.Save()
    End Sub
End Class
