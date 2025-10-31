using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace p5_SerialVisual
{ //hice un cambio

    //hice ootro cambio
    public partial class Form1 : Form//inicia la definicion de la clase
    {

        int[] buffer = new int[200];

        static Bitmap mapaDeBits = new Bitmap(200, 1024);// vamos a pintar aqui
        Graphics graficElements = Graphics.FromImage(mapaDeBits);// erramientas para píntar
        Pen pluma = new Pen(Color.Lime, 2);

        public Form1() // constructor
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            //modificación

            // Suscribir evento de click para recargar puertos
            this.comboBox1.MouseClick += comboBox1_MouseClick;

            // (Opcional) también al desplegar la lista:
            this.comboBox1.DropDown += comboBox1_DropDown;

            // Cargar inicial
            RecargarPuertos();

            //aqui termina

            string[] puertos = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(puertos);

            string[] baudios = { "57600", "78880", "115200" };
            comboBox2.Items.AddRange(baudios);




        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }


        //aqui empieza
        private void RecargarPuertos()
        {
            // Guardar selección actual (si la hay)
            string seleccionado = comboBox1.SelectedItem as string;

            // Obtener y ordenar los puertos disponibles
            string[] puertos = SerialPort.GetPortNames().OrderBy(p => p).ToArray();

            comboBox1.BeginUpdate();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(puertos);
            comboBox1.EndUpdate();

            // Reaplicar selección si sigue existiendo
            if (!string.IsNullOrEmpty(seleccionado) && puertos.Contains(seleccionado))
            {
                comboBox1.SelectedItem = seleccionado;
            }
            else if (puertos.Length > 0)
            {
                // Seleccionar por defecto el primero si no hay selección válida
                comboBox1.SelectedIndex = -1;
            }
            else
            {
                // Si no hay puertos, opcionalmente avisar
                // (Evita mostrar el mensaje una y otra vez si no te conviene)
                // MessageBox.Show("No se detectaron puertos disponibles.", "Aviso",
                //                 MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Si estaba conectado, forzar cierre
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    BtnConneted.Text = "Desconectado";
                    BtnConneted.ForeColor = Color.Red;
                    this.Text = "Visualizador Serial";
                }
            }
        }

        //parte 2

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            RecargarPuertos();
        }



        //aqui termina


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string cadena = serialPort1.ReadExisting();
            string[] partes = cadena.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int valor=0;

            for( int ii = 0; ii < partes.Length; ii++) { 
            for (int i = 0; i < buffer.Length - 1; i++)
            {
                buffer[i] = buffer[i + 1];
            }
            valor = Convert.ToInt32(partes[ii]);
            buffer[buffer.Length - 1] = (1023 - valor) / 2;
        } 




            tbValor.Text = valor.ToString();

            double voltaje = 3.3 * valor / 1023;
            tbVoltaje.Text = voltaje.ToString();

            
            graficElements.Clear(Color.Black);
            for (int i = 0; i < buffer.Length - 1; i++)
            {
                buffer[i] = buffer[i + 1];
                                           //x1  y1  x2  y2
                graficElements.DrawLine(pluma, i, buffer[i], i+1, buffer[i+1]);

            }

            pictureBox1.Image = mapaDeBits;

        }

        private void BtnConneted_Click(object sender, EventArgs e)
        {
            try
            {

                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    if (serialPort1.IsOpen)
                    {
                        BtnConneted.Text = "Conectado";
                        BtnConneted.ForeColor = Color.Green;
                      

                    }

                }
                else {
                    
                        serialPort1.Close();
                    BtnConneted.Text = "Desnectado";
                    BtnConneted.ForeColor = Color.Red;
                   
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el puerto serial, posiblemente esta  otro programa: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string puerto = comboBox1.SelectedItem.ToString();
            serialPort1.PortName = puerto; // Ejemplo: "COM7"
             // Debe coincidir con el NodeMCU


        }

        //inicio
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
           
            RecargarPuertos();
        }
        //fin


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cadenaBaudios = comboBox2.SelectedItem.ToString();
            serialPort1.BaudRate = Convert.ToInt32(cadenaBaudios);   // Debe coincidir con el NodeMCU
            serialPort1.Encoding = System.Text.Encoding.UTF8;


        }
    }
}
