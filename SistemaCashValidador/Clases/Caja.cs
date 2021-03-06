﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaCashValidador.Modelos;

namespace SistemaCashValidador.Clases
{
    class Caja
    {
        private Modelo_Transaccion DB;
        private MessageEventArgs lbComponents;
        private Hashtable data;

        public delegate void lbStoreEventHandler(object sender, MessageEventArgs e);
        public event lbStoreEventHandler lbStoreEvent;
        
        public Caja()
        {
            this.DB = new Modelo_Transaccion();
            this.lbComponents = new MessageEventArgs();
        }

        public Hashtable setCashBoxInitial()
        {
            this.data = this.DB.getCashBox();
            this.lbComponents.inputR1 = (int) data["1"];
            this.lbComponents.inputR2 = (int) data["2"];
            this.lbComponents.inputR5 = (int) data["5"];
            this.lbComponents.inputR10 = (int) data["10"];
            this.lbComponents.inputR20 = (int) data["20"];
            this.lbComponents.inputR50 = (int) data["50"];
            this.lbComponents.inputR100 = (int) data["100"];
            this.lbComponents.inputR200 = (int) data["200"];
            this.lbComponents.inputR500 = (int) data["500"];

            lbStoreEvent(this,lbComponents);

            return this.data;
        }

        public void update(Hashtable data)
        {
            this.DB.setCashBox(data);
            this.setCashBoxInitial();
        }

        public Hashtable getCashBox()
        {            
            return this.data;
        }
    }
}
