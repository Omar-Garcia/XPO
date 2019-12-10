﻿using DevExpress.Xpo;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraGrid.Views.Grid;
using WinFormsApplication.Forms;
using XpoTutorial;

namespace WinFormsApplication {
    public partial class OrdersListForm : RibbonForm {
        readonly DocumentManager documentManager;
        public OrdersListForm(DocumentManager documentManager) {
            InitializeComponent();
            this.documentManager = documentManager;
        }
        private void OrdersGridView_RowClick(object sender, RowClickEventArgs e) {
            if(e.Clicks == 2) {
                e.Handled = true;
                int orderID = (int)OrdersGridView.GetRowCellValue(e.RowHandle, colOid);
                ShowEditForm(orderID);
            }
        }

        private void ShowEditForm(int? orderID) {
            var form = new EditOrderForm(orderID);
            var document = documentManager.View.AddDocument(form);
            form.FormClosed += (s, e) => {
                if(form.OrderID.HasValue) {
                    Reload();
                    OrdersGridView.FocusedRowHandle = OrdersGridView.LocateByValue("Oid", form.OrderID.Value,
                        rowHandle => OrdersGridView.FocusedRowHandle = (int)rowHandle);
                }
            };
        }
        private void Reload() {
            OrdersInstantFeedbackView.Refresh();
        }

        private void BtnNew_ItemClick(object sender, ItemClickEventArgs e) {
            ShowEditForm(null);
        }

        private void BtnDelete_ItemClick(object sender, ItemClickEventArgs e) {
            using(Session session = new Session()) {
                object orderId = OrdersGridView.GetFocusedRowCellValue(colOid);
                Order order = session.GetObjectByKey<Order>(orderId);
                session.Delete(order);
            }
            Reload();
        }

        private void OrdersInstantFeedbackView_ResolveSession(object sender, ResolveSessionEventArgs e) {
            e.Session = new Session();
        }

        private void OrdersInstantFeedbackView_DismissSession(object sender, ResolveSessionEventArgs e) {
            e.Session.Session.Dispose();
        }
    }
}
