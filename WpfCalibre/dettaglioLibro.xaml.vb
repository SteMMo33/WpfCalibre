Imports WpfCalibre
Imports WpfCalibre.Libro


Public Class dettaglioLibro

    Dim _libro As Libro


    'Friend Shared 
    Public Sub SetLibro(ByRef libro As Libro)
        _libro = libro
    End Sub


    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Close()
    End Sub


    ''' <summary>
    ''' Riempie la form con i datiricevuti
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub dettaglioLibro_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Sicurezza
        If IsNothing(_libro) Then Return

        If Not IsNothing(_libro.titolo) Then textBoxTitolo.Text = _libro.titolo

        For Each autore In _libro.autori
            lbAuthors.Items.Add(autore)
        Next

        If Not IsNothing(_libro.tags) Then
            For Each sTag As String In _libro.tags
                lbTags.Items.Add(sTag)
            Next
        End If

        If Not IsNothing(_libro.publisher) Then textPublisher.Text = _libro.publisher
        If Not IsNothing(_libro.desc) Then textDesc.NavigateToString(_libro.desc)

    End Sub

End Class
