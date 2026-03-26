using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class InvoiceHtmlBuilder
    {
        public static string Build(InvoiceViewModel model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<html>
              <head> 
              <meta charset='utf-8'>
              <style>
               body{
                     font-family:Arial,Helvetice,sans-serif;
                     font-size:12px;
                     
                   }

               h2{
                   margin-bottom:5px;                  
                 }
               .header{
                        margin-bottom:20px;
                      }
               .customer{
                          margin-bottom:20px;
                        }
                table{
                        width:100%; border-collapse:collapse; 
                     }
                th, td{
                        border:1px; solid #ccc;
                        padding:5px;
                        text-align:center;
                      }
                th{
                    background:#f2f2f2;
                  }
                .right{
                        text-align:right;
                      }


              </style>
              </head>
              <body>");
            sb.AppendFormat(@"
                              
                             <div class='header'>
                             <h2>BILL</h2>
                             <p>No: {0} </br>
                                Date: {1:dd.MM.yyyy}
                             </p>    
                             </div>

                           ",model.InvoiceNumber,model.InvoiceDate);
            sb.AppendFormat(@"
                              
                            <div class='customer'>
                               <b>Customer Informations</b> </br>
                                {0}</br>
                                {1}
                            </div>

                            ",model.CustomerName,model.CustomerAddress);
            sb.Append(@"

                       <table>
                        <thead>
                        <tr>
                         <th>Title</th>
                         <th>Description</th>
                         <th class='right'>Quantity</th>
                         <th class='right'>Unit Price</th>
                         <th class='right'>Total</th>
                        </tr>
                        </thead>
                       <tbody> 

                      ");
            foreach (var item in model.Items)
            {
                var lineTotal = item.Quantity * item.UnitPrice;

                sb.AppendFormat(@"
                                    
                                    <tr>
                                      <td>{0}</td>
                                      <td>{1}</td>
                                      <td class='right'>{2}</td>
                                      <td class='right'>{3:N2}</td>
                                      <td class='right'>{4:N2}</td>
                                    </tr>",item.Title,item.Description,item.Quantity,item.UnitPrice,lineTotal);
            }

            sb.AppendFormat(@"
                              
                             <tr>
                              <td colspan='4' class='right'><b>Total</b></td>
                              <td class='right'><b>{0:N2}</b></td>
                             </tr>",model.Total);
            sb.Append(@"

                        </tbody>
                        </table>
                        
                         <p style='margin-top:30px font-size:11px;'>
                            This document is electronically generated and does not require a signature.
                         </p>
                      </body>
                    </html>");

            return sb.ToString();

        }
    }
}
