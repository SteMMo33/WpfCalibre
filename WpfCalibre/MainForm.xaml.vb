
Imports Microsoft.Win32
Imports System.Xml.Linq


Class Window1


    Dim libri As List(Of Libro)


    ''' <summary>
    ''' Ricerca per titolo
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Query_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnQuery.Click

        Dim searchTitle As String = tbTitle.Text
        Dim searchAuthor As String = tbAuthor.Text
        Dim searchTag As String = tbTag.Text

        'Controllo che non siano tutte vuote
        If String.IsNullOrEmpty(searchTitle) And String.IsNullOrEmpty(searchAuthor) And String.IsNullOrEmpty(searchTag) Then
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
            ElseIf String.IsNullOrEmpty(searchTag) Then
                ' Create the query per tag
                books = From c In XElement.Load(btnFile.Content.ToString).Elements("record")
                        Where c.Element("tags").ToString.ToUpper.Contains(searchTag.ToUpper) 'Select c.Title, c.authors
            Else

            End If
        Catch ex As Exception
            MsgBox("Errore grave: " & ex.Message, MsgBoxStyle.Critical)
            Return
        End Try

        'Cancella la lista risultati
        ListBox1.Items.Clear()

        Dim text As String
        Dim count As Integer = 0
        Dim item As Object
        Dim element As XElement

        'Legge le impostazioni
        Dim addAuthor As Boolean = CheckBoxAuthors.IsChecked
        Dim addFormats As Boolean = CheckBoxFormats.IsChecked
        Dim addTags As Boolean = CheckBoxTags.IsChecked

        ' Execute the query
        If books IsNot Nothing Then

            libri = New List(Of Libro)

            For Each book In books
                count += 1

                'Costruisce la lista di obggetti libro
                Dim libro As New Libro
                libro.titolo = book.Element("title").Value

                libro.autori = New List(Of String)
                Dim elAutore As XElement = book.Element("authors")
                For Each author In elAutore.Descendants
                    libro.autori.Add(author.Value)
                Next

                libro.tags = New List(Of String)
                element = book.Element("tags")
                If element IsNot Nothing Then
                    For Each sTag In element.Descendants
                        libro.tags.Add(sTag.Value)
                    Next
                End If

                'Descrizione
                element = book.Element("")
                If element IsNot Nothing Then
                    libro.desc = element.Value
                End If

                libri.Add(libro)

                'Costruisce la riga a video
                text = count.ToString + " - " + book.Element("title").Value
                If (addAuthor) Then
                    elAutore = book.Element("authors")
                    For Each author In elAutore.Descendants
                        text += " - " + author.Value
                    Next
                End If
                If (addFormats) Then text += " - " + book.Element("_formats").Value
                'Tags
                If (addTags) Then
                    item = book.Element("tags")
                    If Not IsNothing(item) Then
                        text += " - " + item.Value
                    Else
                        text += " - NoTags"
                    End If
                End If

                ListBox1.Items.Add(text)
            Next
        Else
            ListBox1.Items.Add("Problema creazione oggetto 'books'")
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

        Title = Title + " - CLR: " + System.Windows.Application.ResourceAssembly.ImageRuntimeVersion

    End Sub


    ''' <summary>
    ''' Sulla chiusura avviene il salvataggio delle impostazioni a video e ampiezza finestra
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        '
        My.Settings.ShowAuthors = CheckBoxAuthors.IsChecked
        My.Settings.ShowFormats = CheckBoxFormats.IsChecked
        My.Settings.ShowTags = CheckBoxTags.IsChecked
        '
        My.Settings.WindowSize = New System.Drawing.Size(Me.Width, Me.Height)

        My.Settings.Save()
    End Sub

    Private Sub btnEraseTitle_Click(sender As Object, e As RoutedEventArgs) Handles btnEraseTitle.Click
        tbTitle.Text = ""
    End Sub

    Private Sub btnEraseAuthor_Click(sender As Object, e As RoutedEventArgs) Handles btnEraseAuthor.Click
        tbAuthor.Text = ""
    End Sub

    ''' <summary>
    ''' Doppio click su una riga della lista libri -> apre la finestra di dettaglio
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox1_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles ListBox1.MouseDoubleClick
        Dim index = ListBox1.SelectedIndex

        If index = ListBox1.Items.Count Then 'Doppio click sull'unica riga che indica 0 risultati
            Return
        End If

        Dim dettaglioFrm As New dettaglioLibro
        dettaglioFrm.SetLibro(libri(index))
        dettaglioFrm.ShowDialog()
    End Sub

End Class
