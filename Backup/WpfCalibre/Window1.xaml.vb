
Imports Microsoft.Win32
Imports System.Xml.Linq


Class Window1


    Private Sub Query_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnQuery.Click

        Dim search As String = tbTitle.Text
        If String.IsNullOrEmpty(search) Then
            MsgBox("Testo non definito", MsgBoxStyle.Critical)
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



    Private Sub btnFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFile.Click

        Dim dlg As New OpenFileDialog
        dlg.Filter = "XML files (*.xml)|*.xml|Tutti i files (*.*)|*.*"
        If True = dlg.ShowDialog Then
            btnFile.Content = dlg.FileName
        End If

    End Sub


End Class
