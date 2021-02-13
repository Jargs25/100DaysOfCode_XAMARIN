using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace _100DaysOfCode_Xamarin
{
    public class App : Application
    {
        private MediaFile mFile;
        private ObservableCollection<producto> lstProductos;
        private ObservableCollection<producto> lstProductos2;

        private ICommand icAgregarProducto => new Command(AgregarProducto);
        private ICommand icModificarProducto => new Command(ModificarProducto);
        private ICommand icEliminarProducto => new Command(EliminarProducto);
        private ICommand icBuscarProducto => new Command(BuscarProducto);

        private NavigationPage npPrincipal;
        private Image imgProducto;
        private Button btnSubirImagen;
        private ListView lvRegistros;
        private Entry entCodigo;
        private Entry entNombre;
        private Entry entCantidad;
        private Entry entPrecio;
        private producto Selected;
        private Button btnAgregar;
        private Button btnModificar;
        private Button btnEliminar;
        private Button btnBuscar;

        public App()
        {
            CreateProductos();
            // The root page of your application
            var content = new ContentPage
            {
                Title = "CRUD productos"
            };

            var slContenedor = new StackLayout();
            slContenedor.VerticalOptions = new LayoutOptions()
            {
                Alignment = LayoutAlignment.Start
            };

            Grid formulario = GetFormulario();
            Grid botones = GetBotones();
            Grid lista = GetLista();

            lvRegistros = new ListView
            {
                ItemTemplate = GetTemplate(),
                ItemsSource = lstProductos,
                RowHeight = 100
            };

            lvRegistros.ItemSelected += LvRegistros_ItemSelected;
            lvRegistros.SeparatorColor = Color.Pink;
            lvRegistros.VerticalOptions = LayoutOptions.Start;
            lvRegistros.MinimumHeightRequest = 100;
            lvRegistros.HasUnevenRows = true;

            lista.Children.Add(lvRegistros);
            slContenedor.Children.Add(formulario);
            slContenedor.Children.Add(botones);
            slContenedor.Children.Add(lista);
            content.Content = slContenedor;

            npPrincipal = new NavigationPage(content);
            npPrincipal.BarBackgroundColor = Color.Red;

            MainPage = npPrincipal;
        }

        private async void BtnSubirImagen_Clicked(object sender, EventArgs e)
        {
            IniciarAnimacion(btnSubirImagen);
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Current.MainPage.DisplayAlert("Mensaje de app", "Función no soportada", "Aceptar");
                return;
            }
            mFile = await CrossMedia.Current.PickPhotoAsync();
            if (mFile == null)
            {
                return;
            }
            imgProducto.Source = ImageSource.FromStream(() =>
            {
                return mFile.GetStream();
            });
        }

        private void LvRegistros_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Selected = new producto();
            Selected = (producto) e.SelectedItem;
            entCodigo.Text = Selected.codigo;
            entNombre.Text = Selected.nombre;
            entCantidad.Text = Selected.cantidad.ToString();
            entPrecio.Text = Selected.precio.ToString();
            imgProducto.Source = null;
            imgProducto.Source = Selected.imagen;
            btnAgregar.IsEnabled = false;
            btnModificar.IsEnabled = true;
            btnEliminar.IsEnabled = true;
        }

        private void AgregarProducto()
        {
            IniciarAnimacion(btnAgregar);

            if (sonValidos())
            {
                producto oProducto = new producto("0000", entNombre.Text.Trim(), Convert.ToInt32(entCantidad.Text.Trim()), Convert.ToDouble(entPrecio.Text.Trim()), "NoDisponible");
                if (imgProducto.Source != null)
                {
                    oProducto.imagen = imgProducto.Source;
                }
                lstProductos.Add(oProducto);
                limpiarCampos();
            }
            else
            {
                Current.MainPage.DisplayAlert("Mensaje de app","Verifique los campos","Aceptar");
            }
        }
        private void ModificarProducto()
        {
            IniciarAnimacion(btnModificar);

            producto nuevoProducto = new producto(Selected.codigo, entNombre.Text.Trim(), Convert.ToInt32(entCantidad.Text.Trim()), Convert.ToDouble(entPrecio.Text.Trim()), "NoDisponible");
            if (imgProducto.Source != null)
            {
                nuevoProducto.imagen = imgProducto.Source;
            }
            int index = lstProductos.IndexOf(Selected);
            lstProductos.Remove(Selected);
            lstProductos.Insert(index, nuevoProducto);
            lvRegistros.ItemsSource = lstProductos;
            limpiarCampos();
        }
        private async void EliminarProducto()
        {
            IniciarAnimacion(btnEliminar);

            var res =  await Current.MainPage.DisplayAlert("Mensaje de app", "¿Desea eliminar el registro seleccionado?", "SI", "NO");
            if (res)
            {
                lvRegistros.ItemsSource = lstProductos;
                lstProductos.Remove(Selected);
                limpiarCampos();
            }
        }
        private void BuscarProducto()
        {
            IniciarAnimacion(btnBuscar);

            lstProductos2 = new ObservableCollection<producto>();

            for (int i = 0; i < lstProductos.Count; i++)
            {
                producto item = lstProductos[i];
                string nombre = entNombre.Text != null ? entNombre.Text.Trim() : "";
                string cantidad = entCantidad.Text != null ? entCantidad.Text.Trim() : "";
                string precio = entPrecio.Text != null ? entPrecio.Text.Trim() : "";

                if (item.nombre.Contains(nombre) && item.cantidad.ToString().Contains(cantidad) && item.precio.ToString().Contains(precio))
                {
                    lstProductos2.Add(item);
                }
            }

            lvRegistros.ItemsSource = lstProductos2;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }
        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private void CreateProductos()
        {
            lstProductos = new ObservableCollection<producto>();

            //lstProductos.Add(new producto("0001", "producto1", 1, 1.50, "nodisponible"));
            //lstProductos.Add(new producto("0002", "producto2", 2, 2.50, "nodisponible"));
            //lstProductos.Add(new producto("0003", "producto3", 3, 3.50, "nodisponible"));
            //lstProductos.Add(new producto("0004", "producto4", 4, 4.50, "nodisponible"));
        }
        private DataTemplate GetTemplate()
        {
            DataTemplate dt = new DataTemplate(() =>
            {
                StackLayout slContenedor = new StackLayout();
                Image imageProducto = new Image();
                Label lblCodigo = new Label();
                Label lblNombre = new Label();
                Label lblCantidad = new Label();
                Label lblPrecio = new Label();

                Grid content = new Grid()
                {
                    RowDefinitions =
                    {
                        new RowDefinition()
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition() { Width = 150 },
                        new ColumnDefinition()
                    }
                };

                imageProducto.SetBinding(Image.SourceProperty, "imagen");
                imageProducto.Margin = new Thickness(4);
                lblCodigo.SetBinding(Label.TextProperty, "codigo");
                lblNombre.SetBinding(Label.TextProperty, "nombre");
                lblCantidad.SetBinding(Label.TextProperty, "cantidad");
                lblPrecio.SetBinding(Label.TextProperty, "precio");

                content.Children.Add(imageProducto);
                slContenedor.Children.Add(lblCodigo);
                slContenedor.Children.Add(lblNombre);
                slContenedor.Children.Add(lblCantidad);
                slContenedor.Children.Add(lblPrecio);
                content.Children.Add(slContenedor, 1, 0);

                return new ViewCell() { View = content };
            });

            return dt;
        }
        private Grid GetFormulario()
        {
            Grid contenedor = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = 250 }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = 150 },
                    new ColumnDefinition()
                }
            };
            Grid imagenContenedor = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = 180 },
                    new RowDefinition() { Height = 50 }
                },
                ColumnDefinitions = { new ColumnDefinition() }
            };
            Grid formularioContenedor = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = 38 },
                    new RowDefinition() { Height = 38 },
                    new RowDefinition() { Height = 38 },
                    new RowDefinition() { Height = 38 },
                    new RowDefinition()
                },
                ColumnDefinitions = { new ColumnDefinition() }
            };

            imgProducto = new Image();
            imgProducto.Margin = new Thickness(5, 10, 5, 0);
            btnSubirImagen = new Button();
            btnSubirImagen.Text = "Subir Imagen";
            btnSubirImagen.Clicked += BtnSubirImagen_Clicked;
            btnSubirImagen.BackgroundColor = Color.Pink;
            btnSubirImagen.FontAttributes = FontAttributes.Bold;

            entCodigo = new Entry();
            entNombre = new Entry();
            entCantidad = new Entry();
            entPrecio = new Entry();

            entCodigo.IsEnabled = false;
            entCodigo.Placeholder = "Código";
            entNombre.Placeholder = "Nombre";
            entCantidad.Placeholder = "Cantidad";
            entPrecio.Placeholder = "Precio";

            entCodigo.VerticalOptions = LayoutOptions.Center;
            entNombre.VerticalOptions = LayoutOptions.Center;
            entCantidad.VerticalOptions = LayoutOptions.Center;
            entPrecio.VerticalOptions = LayoutOptions.Center;

            entCodigo.WidthRequest = 30;
            entNombre.WidthRequest = 30;
            entCantidad.WidthRequest = 30;
            entPrecio.WidthRequest = 30;

            imagenContenedor.Children.Add(imgProducto);
            imagenContenedor.Children.Add(btnSubirImagen, 0, 1);
            formularioContenedor.Children.Add(entCodigo);
            formularioContenedor.Children.Add(entNombre, 0, 1);
            formularioContenedor.Children.Add(entCantidad, 0, 2);
            formularioContenedor.Children.Add(entPrecio, 0, 3);

            contenedor.Children.Add(imagenContenedor);
            contenedor.Children.Add(formularioContenedor, 1, 0);

            return contenedor;
        }
        private Grid GetBotones()
        {
            Grid contenedor = new Grid()
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition() { Height = 50 },
                    new RowDefinition() { Height = 50 }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(),
                    new ColumnDefinition()
                }
            };

            btnAgregar = new Button();
            btnModificar = new Button();
            btnEliminar = new Button();
            btnBuscar = new Button();

            btnAgregar.Text = "Agregar producto";
            btnModificar.Text = "Modificar producto";
            btnEliminar.Text = "Eliminar producto";
            btnBuscar.Text = "Buscar producto";

            btnAgregar.Command = icAgregarProducto;
            btnModificar.Command = icModificarProducto;
            btnEliminar.Command = icEliminarProducto;
            btnBuscar.Command = icBuscarProducto;

            btnAgregar.BackgroundColor = Color.Pink;
            btnModificar.BackgroundColor = Color.Pink;
            btnEliminar.BackgroundColor = Color.Pink;
            btnBuscar.BackgroundColor = Color.Pink;

            btnAgregar.FontAttributes = FontAttributes.Bold;
            btnModificar.FontAttributes = FontAttributes.Bold;
            btnEliminar.FontAttributes = FontAttributes.Bold;
            btnBuscar.FontAttributes = FontAttributes.Bold;


            btnModificar.IsEnabled = false;
            btnEliminar.IsEnabled = false;

            contenedor.Children.Add(btnAgregar);
            contenedor.Children.Add(btnModificar, 1, 0);
            contenedor.Children.Add(btnEliminar, 0, 1);
            contenedor.Children.Add(btnBuscar, 1, 2, 1, 2);

            return contenedor;
        }
        private Grid GetLista()
        {
            return new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = 198 }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition()
                }
            };
        }

        private bool sonValidos()
        {
            return !string.IsNullOrEmpty(entNombre.Text) && !string.IsNullOrEmpty(entCantidad.Text) && !string.IsNullOrEmpty(entPrecio.Text);
        }
        private void limpiarCampos()
        {
            entCodigo.Text = null;
            entNombre.Text = null;
            entCantidad.Text = null;
            entPrecio.Text = null;
            btnAgregar.IsEnabled = true;
            btnModificar.IsEnabled = false;
            btnEliminar.IsEnabled = false;
            imgProducto.Source = null;
            Selected = new producto();
        }
        private async void IniciarAnimacion(Button btn)
        {
            await btn.FadeTo(0.5, 100);
            await btn.FadeTo(1, 100);
        }
    }
}
