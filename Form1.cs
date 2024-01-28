using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using System.Windows.Forms;

namespace PNG100to10x10icons
{
    public partial class Form1 : Form
    {
        private string pastaImagens = "";
        private string destino = "";
        private int tamanhoImagem = 420;
        private int numColunas = 10;
        private int numLinhas = 10;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        public Form1()
        {
            InitializeComponent();

            // Define o estilo de borda como FixedDialog
            this.FormBorderStyle = FormBorderStyle.FixedSingle; 
            
            // Desabilita o botão de maximizar
            this.MaximizeBox = false;

            // Desabilita as picturebox
            pictureBox3.Visible = false; 
            pictureBox4.Visible = false;

            //this.FormBorderStyle = FormBorderStyle.None;

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 1, 1));
            this.Size = new Size(410, 350);
        }

        // Botão de selecionar a pasta de entrada
        private void button1_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    pastaImagens = dialog.SelectedPath;

                    // Mostrar ícone de verificação na PictureBox3
                    pictureBox3.Visible = true; 
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Arquivos PNG (*.png)|*.png";
                dialog.DefaultExt = "png";

                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    destino = dialog.FileName;

                    // Mostrar ícone de verificação na PictureBox4
                    pictureBox4.Visible = true; 
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(pastaImagens) || string.IsNullOrEmpty(destino))
            {
                MessageBox.Show("Por favor, selecione a pasta das imagens e o destino.");
                return;
            }

            Bitmap imagemResultante = new Bitmap(tamanhoImagem * numColunas, tamanhoImagem * numLinhas);

            string[] nomesImagens = Directory.GetFiles(pastaImagens)
                .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => ExtrairNumero(Path.GetFileNameWithoutExtension(f)))
                .ToArray();

            using (Graphics g = Graphics.FromImage(imagemResultante))
            {
                g.Clear(Color.Transparent);

                for (int i = 0; i < nomesImagens.Length; i++)
                {
                    try
                    {
                        Bitmap imagem = new Bitmap(nomesImagens[i]);
                        g.DrawImage(imagem, new Rectangle(i % numColunas * tamanhoImagem, i / numColunas * tamanhoImagem, tamanhoImagem, tamanhoImagem));
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine($"Imagem {nomesImagens[i]} não encontrada. Pulando.");
                    }
                }
            }

            imagemResultante.Save(destino, System.Drawing.Imaging.ImageFormat.Png);
            MessageBox.Show($"Imagem resultante salva em: {destino}", "Conclusão");

            // Reinicializa as variáveis da pasta após concluir, sendo possivel selecioná-las denovo
            pastaImagens = "";
            destino = "";

            // Ocultar ícone de verificação na PictureBox3 e 4
            pictureBox3.Visible = false; // 
            pictureBox4.Visible = false; 
        }

        private int ExtrairNumero(string nomeImagem)
        {
            int inicio = nomeImagem.IndexOf("(");
            int fim = nomeImagem.IndexOf(")");
            if (inicio != -1 && fim != -1)
            {
                try
                {
                    return int.Parse(nomeImagem.Substring(inicio + 1, fim - inicio - 1));
                }
                catch (FormatException)
                {
                    // Se não conseguir converter para número, retorna infinito
                }
            }
            return int.MaxValue; // Retorna infinito se não encontrar um número entre parênteses
        }

        private void buttonAplicarConfiguracoes_Click(object sender, EventArgs e)
        {
            // Atualizar as variáveis com os valores digitados pelos usuários
            if (int.TryParse(textBoxTamanhoImagem.Text, out int novoTamanhoImagem))
            {
                tamanhoImagem = novoTamanhoImagem;
            }

            if (int.TryParse(textBoxNumColunas.Text, out int novoNumColunas))
            {
                numColunas = novoNumColunas;
            }

            if (int.TryParse(textBoxNumLinhas.Text, out int novoNumLinhas))
            {
                numLinhas = novoNumLinhas;
            }

            MessageBox.Show("Configurações aplicadas com sucesso!");
        }
    }
}
