/*
 引用此文件说明：必须引用ThoughtWorks.QRCode.dll1
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;

namespace AirJoy.FrameWork
{
    class QRCode
    {
         ///<summary>
         /// 引用此文件说明：必须引用ThoughtWorks.QRCode.dll1;
         ///</summary>
         ///<param name="Text"></param>
         ///<returns></returns>
        public static Image createQrImg(String Text)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            String encoding = "AlphaNumeric";//编码方式；
            try
            {
                int scale = 4;//放大；
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                MessageBox.Show("大小出错!");
                return null;
            }
            try
            {
                int version = 7;
                qrCodeEncoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid version !");
                return null;
            }

            string errorCorrect = "M";
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            Image image;
            String data = Text;
            image = qrCodeEncoder.Encode(data);
            return image;
        }
    }
}
