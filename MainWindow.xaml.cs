using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;


namespace PrimerParcial
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        OleDbConnection conn = new OleDbConnection(ConnectionDB.StrConnection());
        private int aciertos = 0;
        private int errores = 0;
        public int Aciertos { get => aciertos; set => aciertos = value; }
        public int Errores { get => errores; set => errores = value; }

        private static readonly Regex _regex = new Regex("[^0-9]+"); //Expresión regular que indica sólo números

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCalificar_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach(RadioButton rb in FindVisualChildren<RadioButton>(window))
            {
                if (rb.Name.Contains("Correct") && rb.IsChecked == true)
                {
                    Aciertos++;
                }
            }

            Errores = 10 - Aciertos;

            MessageBox.Show($"Obtuviste {Aciertos}! respuestas bien y {Errores}! respuestas mal", "Resultado", MessageBoxButton.OK, MessageBoxImage.Information);
            
            //Mensaje de la calificación obtenida
            MessageBox.Show((Aciertos > 7 ? $"Tu calificación es de {Aciertos}!, pasaste de forma satisfactoria" : $"Tu calificación es de {Aciertos}!, lamentablemente no pasaste"), "Calificacion", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            try
            {
                OleDbCommand cmd = ConnectionDB.InsertCalificacionAlumno(conn);

                // Agregando valores del examen
                cmd.Parameters.AddWithValue("@Carrera", txtCarrera.Text);
                cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                cmd.Parameters.AddWithValue("@Calificacion", Aciertos);
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("MM/dd/yyyy"));
                cmd.ExecuteNonQuery();

                //Se imprime en consola los datos almadenados
                RetrieveMultipleResults(conn);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                //Operador Ternario; es igual a utilizar un if y un else
                MessageBox.Show((ex is SqlException) ? ($"SQL error is: {error}!") : ($"Unexpected Error: {error}!"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConnectionDB.DisconnectDataBase(conn);
            }

            //Limpiar todos los campos
            foreach (RadioButton rb in FindVisualChildren<RadioButton>(window))
            {
                rb.IsChecked = false;
            }

            Aciertos = 0;
            Errores = 0;

            foreach (TextBox txt in FindVisualChildren<TextBox>(window))
            {
                txt.Text = "";
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        static void RetrieveMultipleResults(OleDbConnection connection)
        {
            using (connection)
            {
                OleDbCommand command = new OleDbCommand(
                  "SELECT * FROM Calificaciones",
                  connection);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                OleDbDataReader reader = command.ExecuteReader();

                while (reader.HasRows)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}", reader.GetName(0),
                        reader.GetName(1), reader.GetName(2));

                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}", reader.GetInt32(0),
                            reader.GetString(1), reader.GetString(2));
                    }
                    reader.NextResult();
                }
            }
        }

        private void TxtId_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (_regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void TxtId_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space && txtId.IsFocused == true & e.Key == Key.OemPeriod)
            {
                e.Handled = true;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
