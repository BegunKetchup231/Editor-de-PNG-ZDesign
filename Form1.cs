using Ookii.Dialogs.Wpf;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;


namespace PNG100to10x10icons
{
   
    public partial class Form1 : Form
    {

        private SoundPlayer soundPlayer;
        private string pastaImagens = "";
        private string destino = "";
        private int tamanhoImagem = 420;
        private int numColunas = 10;
        private int numLinhas = 10;

        public Form1()
        {
            InitializeComponent();

            // Inicialize o SoundPlayer com o arquivo de som da pasta de recursos
            soundPlayer = new SoundPlayer(Editor_de_PNG_ByZDesign.Properties.Resources.clicknormal1);

            // Define o estilo de borda como FixedDialog
            this.FormBorderStyle = FormBorderStyle.FixedSingle; 
            
            // Desabilita o botão de maximizar
            this.MaximizeBox = false;

            // Desabilita as picturebox
            pictureBox3.Visible = false; 
            pictureBox4.Visible = false;

            this.Size = new Size(410, 350);

            button1.TabStop = false;
            button2.TabStop = false;
            button3.TabStop = false;
            buttonAplicarConfiguracoes.TabStop = false;
            textBoxTamanhoImagem.TabStop = false;
            textBoxNumColunas.TabStop = false;
            textBoxNumLinhas.TabStop = false;

            // Adicionar manipuladores de evento de clique para os botões
            button1.Click += Botao_Click;
            button2.Click += Botao_Click;
            button3.Click += Botao_Click;
            buttonAplicarConfiguracoes.Click += Botao_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Código de inicialização ao iniciar o App

            // Configurar o foco em outro controle (por exemplo, this ou outro controle)
            this.Focus();
        }

        private void Botao_Click(object sender, EventArgs e)
        {
            // Mover o foco para o próximo controle no formulário
            this.SelectNextControl((Control)sender, true, true, true, true);
        }
        private void ExibirDialogoSelecaoPasta()
        {
            // Tocar o som
            soundPlayer.Play();

            var dialogoPasta = new VistaFolderBrowserDialog();

            // Configurações opcionais
            dialogoPasta.Description = "Selecione uma pasta";
            dialogoPasta.UseDescriptionForTitle = true;

            // Exibe o diálogo e verifica se o usuário pressionou OK
            bool? resultado = dialogoPasta.ShowDialog();

            if (resultado.GetValueOrDefault())  // Verifica se o resultado é true ou se é null (cancelado)
            {
                pastaImagens = dialogoPasta.SelectedPath;

                // Mostrar ícone de verificação na PictureBox3
                pictureBox3.Visible = true;
            }
        }

        // Botão de selecionar a pasta de entrada
        private void button1_Click(object sender, EventArgs e)
        {
            ExibirDialogoSelecaoPasta();
        }

        private void button2_Click(object sender, EventArgs e)
        {   
            // Tocar o som
            soundPlayer.Play();

            // Remover o foco do botão
            Focus();

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
            // Tocar o som
            soundPlayer.Play();

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
            // Tocar o som
            soundPlayer.Play();

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
