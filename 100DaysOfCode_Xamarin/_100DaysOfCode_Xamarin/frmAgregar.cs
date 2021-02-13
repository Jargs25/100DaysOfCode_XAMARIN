using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace _100DaysOfCode_Xamarin
{
    public class frmAgregar : ContentPage
    {
        public static List<producto> lstProductos;
        private EntryCell ecCodigo;
        private EntryCell ecNombre;
        private EntryCell ecCantidad;
        private EntryCell ecPrecio;
        private Label lblMensaje;

        public frmAgregar()
        {
            Title = "Agregar producto";
            StackLayout slContenedor = new StackLayout();
            lblMensaje = new Label();

            TableView tvForm = new TableView();
            TableRoot trForm = new TableRoot();
            TableSection tsForm = new TableSection();

            ecCodigo = new EntryCell();
            ecNombre = new EntryCell();
            ecCantidad = new EntryCell();
            ecPrecio = new EntryCell();

            ecCodigo.Label = "Código";
            ecNombre.Label = "Nombre";
            ecCantidad.Label = "Cantidad";
            ecPrecio.Label = "Precio";

            tsForm.Add(ecCodigo);
            tsForm.Add(ecNombre);
            tsForm.Add(ecCantidad);
            tsForm.Add(ecPrecio);

            trForm.Add(tsForm);
            tvForm.Root = trForm;

            Button btnAgregar = new Button();
            btnAgregar.Text = "Agregar";
            btnAgregar.Clicked += BtnAgregar_Clicked;

            slContenedor.Children.Add(lblMensaje);
            slContenedor.Children.Add(tvForm);
            slContenedor.Children.Add(btnAgregar);
            Content = slContenedor;
        }

        private void BtnAgregar_Clicked(object sender, EventArgs e)
        {
            if (sonValidos())
            {
                lstProductos.Add(new producto("0000", ecNombre.Text.Trim(), Convert.ToInt32(ecCantidad.Text.Trim()),Convert.ToDouble(ecPrecio.Text.Trim()), "NoDisponible"));
                limpiarCampos();
                lblMensaje.Text = "Datos agregados";
            }
            else
            {
                lblMensaje.Text = "Verifique los campos";
            }
        }
        private bool sonValidos()
        {
            return !string.IsNullOrEmpty(ecNombre.Text) && !string.IsNullOrEmpty(ecCantidad.Text) && !string.IsNullOrEmpty(ecPrecio.Text);
        }
        private void limpiarCampos()
        {
            ecNombre.Text = null;
            ecCantidad.Text = null;
            ecPrecio.Text = null;
        }
    }
}
