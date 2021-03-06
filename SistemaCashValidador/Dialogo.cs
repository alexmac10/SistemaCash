﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaCashValidador.Constrollers;

namespace SistemaCashValidador
{
    public partial class dialogoCaja : Form
    {
        public delegate void updateCashBoxEventHandler(object sender, EventArgs e, Hashtable data);
        public event updateCashBoxEventHandler cashBoxEvent;
        private Hashtable stored;
        private TextBox input;

        public dialogoCaja()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                stored = new Hashtable();
                stored.Add("1", this.validateInput(inpt1.Text));
                stored.Add("2", this.validateInput(inpt2.Text));
                stored.Add("5", this.validateInput(inpt5.Text));
                stored.Add("10", this.validateInput(inpt10.Text));
                stored.Add("20", this.validateInput(inpt20.Text));
                stored.Add("50", this.validateInput(inpt50.Text));
                stored.Add("100", this.validateInput(inpt100.Text));
                stored.Add("200", this.validateInput(inpt200.Text));
                stored.Add("500", this.validateInput(inpt500.Text));
                cashBoxEvent(sender, e, stored);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void setInput(Hashtable data)
        {
            foreach (DictionaryEntry register in data)
            {
                inpt1.Text = data["1"].ToString();
                inpt2.Text = data["2"].ToString();
                inpt5.Text = data["5"].ToString();
                inpt10.Text = data["10"].ToString();
                inpt20.Text = data["20"].ToString();
                inpt50.Text = data["50"].ToString();
                inpt100.Text = data["100"].ToString();
                inpt200.Text = data["200"].ToString();
                inpt500.Text = data["500"].ToString();
            }

            this.calculateMoney();
            this.calculateBills();
            this.calculateCashBox();
        }

        private int validateInput(string data)
        {

            if (data != "")
            {
                try
                {
                    return Convert.ToInt32(data);
                }
                catch (Exception ex)
                {
                    throw new Exception("No se admiten letras");
                }
            }
            else
            {
                throw new Exception("No puede haber campos vacios.");
            }

        }

        private void definedInput(object sender, EventArgs e)
        {
            this.input = (TextBox)sender;
        }

        private void getValueKeyboard(object sender, EventArgs e)
        {
            string number = input.Text;
            if (number == "0")
            {
                input.Text = "";
                input.Text += ((Button)sender).Text;
            }
            else
            {
                input.Text += ((Button)sender).Text;
            }

            this.calculateMoney();
            this.calculateBills();
            this.calculateCashBox();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            input.Text = "0";
            this.calculateMoney();
            this.calculateBills();
            this.calculateCashBox();
        }

        private void calculateMoney()
        {
            int total = 0;
            total += Int32.Parse(inpt1.Text);
            total += (Int32.Parse(inpt2.Text) * 2);
            total += (Int32.Parse(inpt5.Text) * 5);
            total += (Int32.Parse(inpt10.Text) * 10);

            totalMoney.Text = total.ToString();

        }

        private void calculateBills()
        {
            int total = 0;
            total += (Int32.Parse(inpt20.Text) * 20);
            total += (Int32.Parse(inpt50.Text) * 50);
            total += (Int32.Parse(inpt100.Text) * 100);
            total += (Int32.Parse(inpt200.Text) * 200);
            total += (Int32.Parse(inpt500.Text) * 500);
            totalBill.Text = total.ToString();
        }

        private void calculateCashBox()
        {
            int total = 0;
            total = Int32.Parse(totalMoney.Text) + Int32.Parse(totalBill.Text);
            totalCaja.Text = total.ToString();
        }
    }
}
