using System;
using System.Drawing;
using System.Windows.Forms;

namespace OptionsWinForm
{
    public partial class MainWindow : Form
    {    
        //Инициализация формы
        public MainWindow()
        {
            InitializeComponent();
        }

        #region //Свойство пользовательского цвета
        /// <summary>
        /// Свойство сообщает какой цвет задал пользователь
        /// </summary>
        public Color ResultColor
        {
            get { return this.pictureBox1.BackColor; }
        }
        #endregion

        #region //Обработка нажатия кнопки "OK"
        private void button_ok_Click(object sender, EventArgs e)
        {
            if (!this.pictureBox1.BackColor.IsEmpty)
            {
                this.DialogResult = DialogResult.OK; //Явное задание результата диалога
                this.Close();
            }
            else
            {
                MessageBox.Show("Следует выбрать цвет");
            }
        }
        #endregion

        #region //Обработка нажатия кнопки "Выбор цвета"
        private void button_GetColor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();

            //Может ли пользователь создавать собственные цвета
            MyDialog.AllowFullOpen = true;

            //Отображать ли кнопку справки
            MyDialog.ShowHelp = true;

            //Присваивает выбранный цвет свойству textBox
            if (MyDialog.ShowDialog() == DialogResult.OK)
                pictureBox1.BackColor = MyDialog.Color;
        }
        #endregion
    }
}
