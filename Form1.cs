using System;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace p5_SerialVisual
{
    public partial class Form1 : Form
    {
        int[] buffer = new int[200];
        static Bitmap mapaDeBits = new Bitmap(200, 1024);
        Graphics graficElements = Graphics.FromImage(mapaDeBits);
        Pen pluma = new Pen(Color.Lime, 2);

        // Variables de control de estado
        bool graficar = true; // Si es false, no se dibuja
        bool pausa = false;   // Si es true, no se actualiza la gráfica ni el buffer

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            string[] puertos = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(puertos);

            string[] baudios = { "57600", "78880", "115200" };
            comboBox2.Items.AddRange(baudios);

            // Opcional: seleccionar los primeros valores para evitar que estén vacíos
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;

            if (comboBox2.Items.Count > 0)
                comboBox2.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Aquí puedes agregar código que quieras ejecutar al cargar el formulario
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!graficar) return; // Si Stop está activo, no procesar nada

            string cadena = serialPort1.ReadExisting();
            string[] partes = cadena.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int valor = 0;

            for (int ii = 0; ii < partes.Length; ii++)
            {
                string datoLimpio = partes[ii].Trim();

                if (int.TryParse(datoLimpio, out valor))
                {
                    if (!pausa)
                    {
                        // Mover los valores del buffer a la izquierda
                        for (int i = 0; i < buffer.Length - 1; i++)
                            buffer[i] = buffer[i + 1];

                        // Insertar nuevo valor al final del buffer (ajustado para que el gráfico quede dentro del bitmap)
                        buffer[buffer.Length - 1] = (1023 - valor) / 2;
                    }

                    // Mostrar el valor numérico y voltaje usando Invoke para evitar errores de hilo
                    tbValor.Invoke((MethodInvoker)(() => tbValor.Text = valor.ToString()));

                    double voltaje = 3.3 * valor / 1023;
                    tbVoltaje.Invoke((MethodInvoker)(() => tbVoltaje.Text = voltaje.ToString("0.00")));

                    // Dibujar gráfico
                    graficElements.Clear(Color.Black);

                    for (int i = 0; i < buffer.Length - 1; i++)
                        graficElements.DrawLine(pluma, i, buffer[i], i + 1, buffer[i + 1]);

                    pictureBox1.Invoke((MethodInvoker)(() => pictureBox1.Image = mapaDeBits));
                }
            }
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
                else
                {
                    serialPort1.Close();
                    BtnConneted.Text = "Desconectado";
                    BtnConneted.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el puerto serial, posiblemente está en uso por otro programa: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
                serialPort1.PortName = comboBox1.SelectedItem.ToString();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.SelectedItem.ToString());
                serialPort1.Encoding = Encoding.UTF8;
            }
        }

        // Botón STOP: detiene completamente la lectura y graficado
        private void BtnStop_Click(object sender, EventArgs e)
        {
            graficar = false;
            pausa = false; // Desactivar pausa también
            BtnStop.BackColor = Color.Red;
            BtnPause.BackColor = SystemColors.Control;
        }

        // Botón PAUSE: congela la gráfica, pero sigue leyendo datos
        private void BtnPause_Click(object sender, EventArgs e)
        {
            pausa = !pausa;
            if (pausa)
                BtnPause.BackColor = Color.Yellow;
            else
                BtnPause.BackColor = SystemColors.Control;
        }

        // Botón RESET: limpia el buffer y reinicia la gráfica
        private void BtnReset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;

            graficElements.Clear(Color.Black);
            pictureBox1.Image = mapaDeBits;

            tbValor.Text = "0";
            tbVoltaje.Text = "0.00";

            pausa = false;
            graficar = true;

            BtnPause.BackColor = SystemColors.Control;
            BtnStop.BackColor = SystemColors.Control;
        }
    }
}

