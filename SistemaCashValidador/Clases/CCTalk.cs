﻿using System;
using System.Collections;
using System.Threading;

namespace SistemaCashValidador.Clases
{
    class CCTalk
    {
        private static CCTalk instancia = null;
        public delegate void updateListBillsEventHandler(object sender, MessageEventArgs e);
        public delegate void updateListCoinsEventHandler(object sender, MessageEventArgs e);
        public delegate void updateLbStoreEventHandler(object sender, MessageEventArgs e);
        public delegate void updateLbTransactionEventHandler(object sender, MessageEventArgs e);
        public event updateListBillsEventHandler listBillsEvent;
        public event updateListCoinsEventHandler listConisEvent;
        public event updateLbStoreEventHandler lbStoresEvent;
        public event updateLbTransactionEventHandler lbTransactionEvent;

        private CashLib.HopperAcceptor hopperAcceptor;
        private CashLib.HopperDispenser hopperDispenser;
        private CashLib.BillAcceptor billAcceptor;
        private CashLib.BillDespenser billDespenser;

        private MessageEventArgs components;
        private Error error;
        private int billDesposited;
        private Hashtable stored;

        public CCTalk()
        {
            error = Error.getInstancia();
            hopperAcceptor = new CashLib.HopperAcceptor();
            hopperDispenser = new CashLib.HopperDispenser();
            billAcceptor = new CashLib.BillAcceptor();
            billDespenser = new CashLib.BillDespenser();
            components = new MessageEventArgs();
        }

        public static CCTalk getInstancia()
        {
            if (instancia == null)
            {
                instancia = new CCTalk();
            }
            return instancia;
        }

        public void getStatus()
        {
            string fail = "";

            if (!hopperAcceptor.openConnection())
            {
                fail += " No Conectado Hopper Accepter";
            }

            if (!hopperAcceptor.openConnection())
            {
                fail += " No Conectado Hopper Dispenser";
            }

            if (!billAcceptor.openConnection())
            {
                fail += " No Conectado Bill Acceptor";
            }

            if (!billDespenser.openConnection())
            {
                fail += " No Conectado Bill Dispenser";
            }
            this.error.setMesseg(fail);
        }

        public void setEvents()
        {
            billAcceptor.powerUpEvent += powerUpHandle;
            billAcceptor.connectEvent += connectedHandle;
            billAcceptor.stackEvent += stackHandle;
            billAcceptor.powerUpCompleteEvent += PowerUpCompletedHandle;
            billAcceptor.escrowEvent += escrowHandle;
            billAcceptor.setEvents();
        }

        public void enableDevices(Hashtable DBStored)
        {            
            billAcceptor.enable();
            billDespenser.enable();
            hopperAcceptor.enable();
            hopperDispenser.enable();

            this.stored = DBStored;
            this.billDesposited = 0;
            this.setValuesInitialLabelsAndList();

            listConisEvent(this, components);
            listBillsEvent(this, components);
            lbTransactionEvent(this, components);
        }

        public int getCash(int total)
        {

            byte[] result;
            int deposited = 0;
            int contador = 0;
            this.error.setMesseg("Ingrese el efectivo");
            while (deposited < total)
            {
                result = hopperAcceptor.depositCash();

                if (result[4] != contador)
                {
                    switch (result[5])
                    {
                        case 7:
                            components.lbMoney10 += 1;
                            components.listCoins = 10;
                            deposited += 10;
                            listConisEvent(this, components);
                            this.stored["10"] = components.lbMoney10;
                            break;
                        case 6:
                            components.lbMoney5 += 1;
                            components.listCoins = 5;
                            deposited += 5;
                            listConisEvent(this, components);
                            this.stored["5"] = components.lbMoney5;
                            break;
                        case 5:
                            components.lbMoney2 += 1;
                            components.listCoins = 2;
                            deposited += 2;
                            listConisEvent(this, components);
                            this.stored["2"] = components.lbMoney2;
                            break;
                        case 4:
                            components.lbMoney1 += 1;
                            components.listCoins = 1;
                            deposited += 1;
                            listConisEvent(this, components);
                            this.stored["1"] = components.lbMoney5;
                            break;
                    }
                    components.lbTotal = deposited;
                    lbTransactionEvent(this, components);
                    lbStoresEvent(this, components);
                    contador = result[4];
                }
                else if (this.billDesposited != 0)
                {
                    switch (this.billDesposited)
                    {
                        case 20:
                            components.lbBill20 += 1;
                            components.listBills = 20;
                            listBillsEvent(this, components);
                            this.stored["20"] = components.lbBill20;
                            break;
                        case 50:
                            components.lbBill50 += 1;
                            components.listBills = 50;
                            listBillsEvent(this, components);
                            this.stored["50"] = components.lbBill50;
                            break;
                        case 100:
                            components.lbBill100 += 1;
                            components.listBills = 100;
                            listBillsEvent(this, components);
                            this.stored["100"] = components.lbBill100;
                            break;
                        case 200:
                            components.lbBill200 += 1;
                            components.listBills = 200;
                            listBillsEvent(this, components);
                            this.stored["200"] = components.lbBill200;
                            break;
                        case 500:
                            components.lbBill500 += 1;
                            components.listBills = 500;
                            listBillsEvent(this, components);
                            this.stored["500"] = components.lbBill500;
                            break;
                    }

                    lbStoresEvent(this, components);
                    deposited += this.billDesposited;
                    components.lbTotal = deposited;
                    lbTransactionEvent(this, components);
                    this.billDesposited = 0;

                }                
            }
            this.error.setMesseg("Transacción terminada");
            return deposited;
        }

        public void disableDevices()
        {            
            hopperAcceptor.disable();
            hopperDispenser.disable();
            billAcceptor.disable();
            billDespenser.disable();

        }

        private double getBillDeposite(double bill)
        {
            return bill;
        }

        private void setValuesInitialLabelsAndList()
        {
            this.components.listCoins = 0;
            this.components.listBills = 0;
            this.components.lbTotal = 0;
            this.components.lbMoney1 = (int)this.stored["1"];
            this.components.lbMoney2 = (int)this.stored["2"];
            this.components.lbMoney5 = (int)this.stored["5"];
            this.components.lbMoney10 = (int)this.stored["10"];
            this.components.lbBill20 = (int)this.stored["20"];
            this.components.lbBill50 = (int)this.stored["50"];
            this.components.lbBill100 = (int)this.stored["100"];
            this.components.lbBill200 = (int)this.stored["200"];
            this.components.lbBill500 = (int)this.stored["500"];
        }

        public Hashtable getCashStored()
        {
            return this.stored;
        }

        public void setDeliverCash(int cash, Hashtable countCash)
        {
            string valor = "";
            components.lbCambio = cash;
            lbTransactionEvent(this, components);

            this.error.setMesseg("Entregando cambio .... : " + cash);

            foreach (DictionaryEntry i in countCash)
            {
                int key = (int)i.Key;
                int value = (int)i.Value;
                if (key == 10 || key == 5 || key == 1)
                {
                    this.stored[key.ToString()] = (int)this.stored[key.ToString()] - 1;
                    this.updateComponents(key, value);
                    hopperDispenser.returnCash(key, value);
                }
                else if (key == 20 || key == 50 || key == 100)
                {
                    this.error.setMesseg("Entregando billetes : " + cash + " : " + key + " : " + value);
                    this.stored[key.ToString()] = (int)this.stored[key.ToString()] - 1;
                    this.updateComponents(key, value);                    
                    billDespenser.returnCash(key, value);
                }
                lbStoresEvent(this, components);
                valor += i.Key.ToString() + " : " + i.Value.ToString() + "  ";
            }

            this.error.setMesseg("Transacción terminada");
        }

        private void updateComponents(int value, int count)
        {
            switch (value)
            {
                case 1:
                    components.lbMoney1 -= count;
                    break;                                    
                case 5:
                    components.lbMoney5 -= count;
                    break;
                case 10:
                    components.lbMoney10 -= count;
                    break;
                case 20:
                    components.lbBill20 -= count;
                    break;
                case 50:
                    components.lbBill50 -= count;
                    break;
                case 100:
                    components.lbBill100 -= count;
                    break;
                case 200:
                    components.lbBill200 -= count;
                    break;
                case 500:
                    components.lbBill500 -= count;
                    break;
            }
        }

        #region Eventos para Bill Accetor

        private void powerUpHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Manejador de POWER UP");
        }

        private void connectedHandle(object sender, EventArgs e)
        {
            billAcceptor.configEnable();            
        }

        private void stackHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : Stack");
            this.billDesposited = (int)billAcceptor.getDepositeBill();
            Console.WriteLine("Recibido : {0}", this.billDesposited);
        }

        private void PowerUpCompletedHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : POWERUP_COMPLETED");
        }

        private void escrowHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : ESCROW");
        }

        #endregion

    }
}