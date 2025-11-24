using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestorProductosWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GestorProductos gestor = new GestorProductos();
        public MainWindow()
        {
            InitializeComponent();
            CargarDatosIniciales();

            dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();

            comboTipoBusqueda.Items.Add("ID");
            comboTipoBusqueda.Items.Add("Nombre");
            comboTipoBusqueda.SelectedIndex = 0;

            comboCriterioOrden.Items.Add("ID");
            comboCriterioOrden.Items.Add("Precio");
            comboCriterioOrden.Items.Add("Nombre");
            comboCriterioOrden.SelectedIndex = 0;

        }

        private void CargarDatosIniciales()
        {
            gestor.AgregarProducto(new Producto 
            { 
                Id = 3, 
                Nombre = "Teclado", 
                CodigoBarras = "789456",
                Categoria = "Electrónica", 
                Precio = 300, 
                Stock = 20 
            });
            gestor.AgregarProducto(new Producto
            {
                Id = 15,
                Nombre = "Computadora",
                CodigoBarras = "123456",
                Categoria = "Electrónica",
                Precio = 12000,
                Stock = 10
            });
            gestor.AgregarProducto(new Producto
            {
                Id = 1,
                Nombre = "Bocina",
                CodigoBarras = "147852",
                Categoria = "Electrónica",
                Precio = 4000,
                Stock = 2
            });
            gestor.AgregarProducto(new Producto
            {
                Id = 5,
                Nombre = "USB",
                CodigoBarras = "258963",
                Categoria = "Electrónica",
                Precio = 15,
                Stock = 30
            });
            gestor.AgregarProducto(new Producto
            {
                Id = 9,
                Nombre = "Sudadera",
                CodigoBarras = "369258",
                Categoria = "Ropa",
                Precio = 300,
                Stock = 15
            });
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string criterio = comboTipoBusqueda.SelectedItem.ToString();
            string valor = txtBusqueda.Text;

            //La lista ordenada

            List<Producto> productosParaBusqueda = new List<Producto>(gestor.ObtenerListaProductos());
            OrdenadorSimplificado.QuickSortPorId(productosParaBusqueda);

            switch (criterio)
            {
                case "ID":
                    if (int.TryParse(valor, out int id))
                    {
                        var (producto, iteraciones) = BuscadorSimplificado.BusquedaBinariaPorId(productosParaBusqueda, id);
                        MostrarResultado(producto, id);
                    }
                    break;
                case "Nombre":
                    var (productoNombre, iteracionesNombre) = BuscadorSimplificado.BusquedaSecuencialNombre(productosParaBusqueda, valor);
                    MostrarResultado(productoNombre, iteracionesNombre);
                    break;

            }
        }
        private void MostrarResultado(Producto producto, int iteraciones)
        {
            txtResultadoBusqueda.Text = producto?.ToString() ?? "No encontrado";
            progressIteraciones.Value = iteraciones * 5;
        }
        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var ventanaAgregar = new AgregarProductoWindow();
            if(ventanaAgregar.ShowDialog() == true)
            {
               Producto nuevoProducto = ventanaAgregar.Producto;
                try
                {
                    gestor.AgregarProducto(nuevoProducto);
                    dataGridProductos.ItemsSource = null;
                    dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if(dataGridProductos.SelectedItem is Producto productoSeleccionado)
            {
                bool eliminado = gestor.EliminarProductoPorCodigo(productoSeleccionado.CodigoBarras);
                if (eliminado)
                {
                    dataGridProductos.ItemsSource = null;
                    dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();
                    MessageBox.Show("Producto eliminado correctamente.", "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnOrdenar_Click(object sender, RoutedEventArgs e)
        {
            List<Producto> productosParaOrdenar = new List<Producto>(gestor.ObtenerListaProductos());
            string criterio = comboCriterioOrden.SelectedItem.ToString();

            switch (criterio)
            {
                case "ID":
                    OrdenadorSimplificado.QuickSortPorId(productosParaOrdenar);
                    break;
                case "Precio":
                    OrdenadorSimplificado.QuickSortPorPrecio(productosParaOrdenar);
                    break;
                case "Nombre":
                    OrdenadorSimplificado.MergeSortPorNombre(productosParaOrdenar);
                    break;
            }
            listViewOrdenados.ItemsSource = productosParaOrdenar;
            DibujarGraficoBarras(productosParaOrdenar);
        }

        private void DibujarGraficoBarras(List<Producto> productos)
        {
            canvasGrafico.Children.Clear();
            double maxPrecio = (double)productos.Max(p => p.Precio);
            double escala = canvasGrafico.ActualHeight / maxPrecio;

            for (int i = 0; i < productos.Count; i++)
            {
                Rectangle barra = new Rectangle
                {
                    Width = 30,
                    Height = (double)productos[i].Precio * escala,
                    Fill = Brushes.Blue,
                    Margin = new Thickness(i * 40, canvasGrafico.ActualHeight - ((double)productos[i].Precio * escala), 0, 0)
                };
                
                canvasGrafico.Children.Add(barra);
                TextBlock etiqueta = new TextBlock
                {
                    Text = productos[i].Nombre,
                    Margin = new Thickness(i * 40, canvasGrafico.ActualHeight - 20, 0, 0)
                };
                canvasGrafico.Children.Add(etiqueta);
            }
        }
    }
}