using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Lab6_7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        

        int width_image;
        int height_image;

        byte[,] matrix;

        byte[,] even_encode;

        byte[,] ImageToBytes(Bitmap image, byte[,] matrix) // Изменение: Добавил byte[,] matrix
        {
            matrix = new byte[LoadPicture.Image.Width, LoadPicture.Image.Height];

          Color[,] colors = new Color[LoadPicture.Image.Width, LoadPicture.Image.Height]; // сделал локальной переменной
            for (int i = 0; i < (LoadPicture.Image.Width); i++)
            {
                for (int j = 0; j < (LoadPicture.Image.Height); j++)
                {
                    colors[i, j] = image.GetPixel(i, j);
                    matrix[i, j] = colors[i, j].R;
                }
            }
            return matrix;
        }
        void NewImage(byte[,] matrix, string name_image, string folder)
        {
            Bitmap new_image = new Bitmap(LoadPicture.Image.Width, LoadPicture.Image.Height);

            string path = @"E:\Теория информации и кодирования\Результаты6-7\"; // Сделал локальной переменной

            for (int x = 0; x < LoadPicture.Image.Width; x++)
                for (int y = 0; y < LoadPicture.Image.Height; y++)
                {
                    int color = matrix[x, y];
                    Color pixel = Color.FromArgb(255, color, color, color);

                    new_image.SetPixel(x, y, pixel);
                }

            new_image.Save($@"{path}{folder}\{name_image}.png", System.Drawing.Imaging.ImageFormat.Jpeg);
        }


        byte[,] Seven_Bit(byte[,] matrix) // Изменение: Добавил byte[,] matrix
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    matrix[i, j] &= 0xFE;
                }
            }
            return matrix;
        }
        byte[,] Four_Bit(byte[,] matrix) // Изменение: Добавил byte[,] matrix
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    matrix[i, j] &= 0xF0;
                }
            }
            return matrix;
        }



        public byte[,] Hemming_Table_Encode(byte[,] matrix)
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int k = 0; k < height_image; k++)
                {
                    List<int> list_byte = new List<int>();

                    byte[] buffer = new byte[1];
                    buffer[0] = matrix[i, k];

                    BitArray bit_arr = new BitArray(buffer);
                    for (int j = bit_arr.Count - 4; j < bit_arr.Count; j++)
                    {
                        if (bit_arr.Get(j))
                            list_byte.Add(1);
                        else list_byte.Add(0);
                    }
                    int e1 = (list_byte[1] & 1) ^ (list_byte[2] & 1) ^ (list_byte[3] & 1);
                    int e2 = (list_byte[0] & 1) ^ (list_byte[2] & 1) ^ (list_byte[3] & 1);
                    int e3 = (list_byte[0] & 1) ^ (list_byte[1] & 1) ^ (list_byte[3] & 1);

                    matrix[i, k] = 0;
                    for (int j = 0; j < list_byte.Count; j++)
                    {
                        if (list_byte[j] == 1)
                        {
                            matrix[i, k] += (byte)Math.Pow(2, j + 4);
                        }
                    }
                    if (e1 == 1)
                    {
                        matrix[i, k] += (byte)Math.Pow(2, 3);
                    }
                    if (e2 == 1)
                    {
                        matrix[i, k] += (byte)Math.Pow(2, 2);
                    }
                    if (e3 == 1)
                    {
                        matrix[i, k] += (byte)Math.Pow(2, 1);
                    }
                }
            }
            return matrix;
        }
        public byte[,] Hemming_Table_Decode(byte[,] matrix)
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    matrix[i, j] &= 0xF0;
                }
            }
            return matrix;
        }
        public byte[,] Hemming_No_Table_Encode(byte[,] matrix)
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int k = 0; k < height_image; k++)
                {
                    List<int> list_byte = new List<int>();
                    byte[] buffer = new byte[1];

                    buffer[0] = matrix[i, k];
                    BitArray bit_arr = new BitArray(buffer);
                    for (int j = bit_arr.Count - 4; j < bit_arr.Count; j++)
                    {
                        if (bit_arr.Get(j))
                        {
                            list_byte.Add(1);
                        }
                        else list_byte.Add(0);
                    }
                    int e1 = (list_byte[1] & 1) ^ (list_byte[2] & 1) ^ (list_byte[3] & 1);
                    int e2 = (list_byte[0] & 1) ^ (list_byte[2] & 1) ^ (list_byte[3] & 1);
                    int e3 = (list_byte[0] & 1) ^ (list_byte[1] & 1) ^ (list_byte[3] & 1);
                    matrix[i, k] = 0;
                    if (e1 == 1)
                    {
                        matrix[i, k] += (byte)Math.Pow(2, 7);
                    }
                    if (e2 == 1)
                    {
                        matrix[i, k] += (byte)Math.Pow(2, 6);
                    }
                    if (list_byte[3] == 1)
                    {
                        matrix[i, k] += (byte)Math.Pow(2, 5);
                    }
                    if (e3 == 1)
                    {
                        matrix[i, k] += (byte)Math.Pow(2, 4);
                    }
                    for (int j = 0; j < list_byte.Count - 1; j++)
                    {
                        if (list_byte[j] == 1)
                        {
                            matrix[i, k] += (byte)Math.Pow(2, j + 1);
                        }
                    }
                }
            }
            return matrix;
        }
        public byte[,] Hemming_No_Table_Decode(byte[,] matrix)
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int k = 0; k < height_image; k++)
                {
                    byte[] buffer = new byte[1];
                    buffer[0] = matrix[i, k];

                    BitArray bit_arr = new BitArray(buffer);
                    List<int> list_bit = new List<int>();

                    for (int j = 0; j < bit_arr.Count; j++)
                    {
                        if (bit_arr.Get(j))
                        {
                            list_bit.Add(1);
                        }
                        else list_bit.Add(0);
                    }
                    int x1 = list_bit[7] ^ list_bit[5] ^ list_bit[3] ^ list_bit[1],
                        x2 = list_bit[6] ^ list_bit[5] ^ list_bit[2] ^ list_bit[1],
                        x3 = list_bit[4] ^ list_bit[3] ^ list_bit[2] ^ list_bit[1];

                    double bit = 0;
                    if (x1 == 1)
                    {
                        bit += Math.Pow(2, 7);
                    }
                    if (x2 == 1)
                    {
                        bit += Math.Pow(2, 6);
                    }
                    if (x3 == 1)
                    {
                        bit += Math.Pow(2, 5);
                    }
                    if (list_bit[1] == 1)
                    {
                        bit += Math.Pow(2, 4);
                    }
                    matrix[i, k] = (byte)bit;
                }
            }
            return matrix;
        }

        int PopCount(byte bit) // Функция PopCount 
        {
            byte[] buff = new byte[1];
            buff[0] = bit;

            BitArray bit_array = new BitArray(buff);
            int count = 0;

            for (int i = 0; i < bit_array.Count; i++)
            {
                if (bit_array.Get(i))
                {
                    count++;
                }
            }

            return count;
        }


        public byte[,] Inverse_Encode(byte[,] matrix)
        {
            byte[,] arr_clone = new byte[width_image, height_image];
            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    int count = PopCount(matrix[i, j]);

                    if (count % 2 != 0)
                    {
                        arr_clone[i, j] = (byte)~matrix[i, j];
                    }
                    else arr_clone[i, j] = matrix[i, j];
                }
            }
            for (int i = 0; i < width_image; i++)
            {
                for (int k = 0; k < height_image; k++)
                {
                    byte[] buffer = new byte[1];
                    buffer[0] = arr_clone[i, k];

                    BitArray bit_arr = new BitArray(buffer);
                    List<char> list_bits = new List<char>();

                    for (int j = bit_arr.Count - 4; j < bit_arr.Count; j++)
                    {
                        if (bit_arr.Get(j))
                        {
                            list_bits.Add('1');
                        }
                        else list_bits.Add('0');
                    }

                    buffer[0] = matrix[i, k];
                    bit_arr = new BitArray(buffer);

                    for (int j = bit_arr.Count - 4; j < bit_arr.Count; j++)
                    {
                        if (bit_arr.Get(j))
                        {
                            list_bits.Add('1');
                        }
                        else list_bits.Add('0');
                    }
                    matrix[i, k] = 0;

                    for (int j = 0; j < list_bits.Count; j++)
                    {
                        if (list_bits[j] == '1')
                        {
                            matrix[i, k] += (byte)Math.Pow(2, j);
                        }
                    }
                }
            }
            return matrix;
        }


        public byte[,] Inverse_Decode(byte[,] matrix)
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    matrix[i, j] &= (byte)0xF0;
                }
            }
            return matrix;
        }


        public void CheckError(byte[,] matrix, byte[,] second_matrix, string name, string second_name, TextBox message)
        {
            int count = 0;

            for (int i = 0; i < LoadPicture.Image.Width; i++)
            {
                for (int j = 0; j < LoadPicture.Image.Height; j++)
                {
                    if (matrix[i, j] == second_matrix[i, j])
                        count++;
                }
            }
            message.Text += $"{name} to {second_name} ";
            if (count == even_encode.Length)
            {
                message.Text += "Успех";
            }
            else message.Text += $"Ошибки: {count} ";
        }

        public void Error_Code(ref byte[,] matrix, int min, int max)
        {
            for (int i = 0; i < width_image; i++)
            {

                if (i < width_image / 2)
                {

                    for (int j = 0; j < height_image; j++)
                    {

                        if (j < height_image / 2)
                        {
                            Create_Error(1, ref matrix[i, j], min, max);
                        }
                        else if (j >= height_image / 2)
                        {
                            Create_Error(3, ref matrix[i, j], min, max);
                        }

                    }

                }
                else if (i >= width_image / 2)
                {
                    for (int j = 0; j < height_image / 2; j++)
                    {
                        Create_Error(2, ref matrix[i, j], min, max);
                    }
                }
            }

        }

        public void Create_Error(int num_er, ref byte bit, int min, int max)
        {
            int[] rand = new int[num_er];

            Random random = new Random(); // сделал локальной переменной

            for (int i = 0; i < num_er; i++)
            {
                rand[i] = random.Next(min, max);

            }
            for (int i = 0; i < num_er; i++)
            {
                bit ^= (byte)(1 << rand[i]);
            }
        }
    

        public byte[,] Even_Encode(byte[,] matrix)
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    int sum = PopCount(matrix[i, j]);
                    if (sum % 2 != 0)
                    {
                        matrix[i, j] += (byte)1;
                    }
                }
            }
            return matrix;
        }
        public byte[,] Even_Decode(byte[,] matrix)
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    matrix[i, j] &= (byte)0xFE;
                }
            }
            return matrix;
        }

        public byte[,] Cycle_Encode(byte[,] matrix)
        {
            byte b1 = (1 << 4) + 1;
            byte b2 = (byte)(b1 << 1);
            byte b3 = (byte)(b2 << 1);
            byte b4 = (byte)(b3 << 1);

            byte[] group = { b1, b2, b3, b4 };

            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    List<int> list_bytes = new List<int>();

                    byte[] buffer_bytes = new byte[1];
                    buffer_bytes[0] = (byte)matrix[i, j];

                    BitArray bit_arr = new BitArray(buffer_bytes);
                    for (int k = 0; k < bit_arr.Count; k++)
                    {
                        if (bit_arr.Get(k))
                        {
                            list_bytes.Add(1);
                        }
                        else
                        {
                            list_bytes.Add(0);
                        }
                    }
                    matrix[i, j] = 0;

                    for (int k = list_bytes.Count - 4; k < list_bytes.Count; k++)
                    {
                        if (list_bytes[k] == 1)
                        {
                            matrix[i, j] += group[k - 4];
                        }
                    }
                }
            }
            return matrix;
        }

        public byte[,] Cycle_Decode(byte[,] matrix)
        {
            for (int i = 0; i < width_image; i++)
            {
                for (int j = 0; j < height_image; j++)
                {
                    matrix[i, j] &= 0xf0;
                }
            }
            return matrix;
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap image = new Bitmap(dialog.FileName); // сделал локальной переменной
                    LoadPicture.Image = image;
                    matrix = ImageToBytes(image, matrix);
                    width_image = LoadPicture.Image.Width;
                    height_image = LoadPicture.Image.Height;
                }
                catch
                {
                    DialogResult message = MessageBox.Show("Ошибка загрузки изображения!", "Загрузка изображения", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        private void WorkWithImageButton_Click(object sender, EventArgs e)
        {
            // matrix, four_bit, seven_bit
            NewImage(matrix, "matrix", "4and7matrix");

            var seven_matrix = Seven_Bit((byte[,])matrix.Clone()); // сделал локальной переменной и изменил byte[,] на var
            NewImage(seven_matrix, "seven_bit", "4and7matrix");

            var four_matrix = Four_Bit((byte[,])matrix.Clone()); // сделал локальной переменной и изменил byte[,] на var
            NewImage(four_matrix, "four_bit", "4and7matrix");

            //Even
            even_encode = Even_Encode((byte[,])seven_matrix.Clone());
            NewImage(even_encode, "Code_Even", "Even");

            var even_decode = Even_Decode(even_encode); // сделал локальной переменной и изменил byte[,] на var
            CheckError(seven_matrix, even_decode, "Make7_bit", "Decode_Evene", textBox1);
            NewImage(even_decode, "Decode_Even", "Even");

            var even_error_code = (byte[,])even_encode.Clone(); // сделал локальной переменной и изменил byte[,] на var
            Error_Code(ref even_error_code, 1, 7);
            NewImage(even_error_code, "Error_Code", "Even");

            var even_error_decode = Even_Decode(even_error_code); //  изменил byte[,] на var
            CheckError(seven_matrix, even_error_decode, "Make7_bit", "Decode_Error_Evene", textBox8);
            NewImage(even_error_decode, "Error_Decode", "Even");

            // Hemming_table
            var four_matrix_hemming_table = Hemming_Table_Encode((byte[,])four_matrix.Clone()); // сделал локальной переменной и изменил byte[,] на var
            NewImage(four_matrix_hemming_table, "Hemming_table", "Hemming_Table");

            var four_matrix_hemming_table_decode = Hemming_Table_Decode((byte[,])four_matrix_hemming_table.Clone()); // сделал локальной переменной и изменил byte[,] на var
            CheckError(four_matrix, four_matrix_hemming_table_decode, "Make4_bit", "Hemming_table_Decode", textBox3);
            NewImage(four_matrix_hemming_table_decode, "Hemming_table_Decode", "Hemming_Table");

            var four_matrix_hemming_table_error = (byte[,])four_matrix_hemming_table.Clone(); //  изменил byte[,] на var
            Error_Code(ref four_matrix_hemming_table_error, 4, 7);
            NewImage(four_matrix_hemming_table_error, "Hemming_table_Error", "Hemming_Table");
            var four_matrix_hemming_table_error_decode = Hemming_Table_Decode(four_matrix_hemming_table_error); //  изменил byte[,] на var

            CheckError(four_matrix, four_matrix_hemming_table_error_decode, "Make4_bit", "Hemming_table_Error_Decode", textBox6);
            NewImage(four_matrix_hemming_table_error_decode, "Hemming_table_Error_Decode", "Hemming_Table");


            // Hemming_no_table
            var four_matrix_hemming_no_table = Hemming_No_Table_Encode((byte[,])four_matrix.Clone()); // сделал локальной переменной и изменил byte[,] на var
            NewImage(four_matrix_hemming_no_table, "Hemming_no_table", "Hemming_No_Table");

            var four_matrix_hemming_no_table_decode = Hemming_No_Table_Decode((byte[,])four_matrix_hemming_no_table.Clone()); // сделал локальной переменной и изменил byte[,] на var
            CheckError(four_matrix, four_matrix_hemming_no_table_decode, "Make4_bit", "Hemming_no_table_Decode", textBox4);
            NewImage(four_matrix_hemming_no_table_decode, "Hemming_no_table_Decode", "Hemming_No_Table");

            var four_matrix_hemming_no_table_error = (byte[,])four_matrix_hemming_no_table.Clone(); //  изменил byte[,] на var
            Error_Code(ref four_matrix_hemming_no_table_error, 4, 7);
            NewImage(four_matrix_hemming_no_table_error, "Hemming_no_table_Error", "Hemming_No_Table");

            var four_matrix_hemming_no_table_error_decode = Hemming_No_Table_Decode((byte[,])four_matrix_hemming_no_table_error.Clone()); //  изменил byte[,] на var
            CheckError(four_matrix, four_matrix_hemming_no_table_error_decode, "Make4_bit", "Hemming_no_table_Error_Decode", textBox5);
            NewImage(four_matrix_hemming_no_table_error_decode, "Hemming_no_table_Error_Decode", "Hemming_No_Table");

            //Inverse
            var inverse_encode = Inverse_Encode((byte[,])four_matrix.Clone()); // сделал локальной переменной и изменил byte[,] на var
            NewImage(inverse_encode, "Inverse_Code", "Inverse");

            var inverse_decode = Inverse_Decode(inverse_encode); // сделал локальной переменной и изменил byte[,] на var
            CheckError(four_matrix, inverse_decode, "Make4_bit", "Inverse_Decode", textBox2);
            NewImage(inverse_decode, "Inverse_Decode", "Inverse");

            var inverse_error_code = (byte[,])inverse_encode.Clone(); //  изменил byte[,] на var
            Error_Code(ref inverse_error_code, 4, 7);

            var Inverse_Error_Decode = Inverse_Decode(inverse_error_code); // изменил byte[,] на var
            CheckError(four_matrix, Inverse_Error_Decode, "Make4_bit", "Inverse_Decode_Error", textBox7);

            NewImage(inverse_error_code, "Inverse_Error_Code", "Inverse");
            NewImage(Inverse_Error_Decode, "Inverse_Error_Decode", "Inverse");

            //Cycle
            var four_matrix_cycle = Cycle_Encode((byte[,])four_matrix.Clone()); // сделал локальной переменной и изменил byte[,] на var
            NewImage(four_matrix_cycle, "Cycle", "Cycle");

            var four_matrix_cycle_decode = Cycle_Decode((byte[,])four_matrix_cycle.Clone()); // сделал локальной переменной и изменил byte[,] на var
            CheckError(four_matrix, four_matrix_cycle_decode, "Make4_bit", "Cycle_Decode", textBox9);
            NewImage(four_matrix_cycle_decode, "Cycle_Decode", "Cycle");

            var cycle_error = (byte[,])four_matrix_cycle.Clone(); //  изменил byte[,] на var
            Error_Code(ref cycle_error, 4, 7);
            NewImage(cycle_error, "Cycle_Error", "Cycle");

            var cycle_error_decode = (byte[,])cycle_error.Clone(); //  изменил byte[,] на var
            CheckError(cycle_error, cycle_error_decode, "Make4_bit", "Cycle_Error_Decode", textBox10);
            NewImage(cycle_error_decode, "Cycle_Error_Decode", "Cycle");
        }
    }


}



