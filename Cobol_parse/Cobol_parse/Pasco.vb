Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Text
Imports System.Net
Imports System.Xml

Module PascoCounty
    Sub ParsePascoCounty()

        Dim Pasco_County_record_length As Integer = 118616
        'variables
        Dim Pasco_County_Certificate_Number(Pasco_County_record_length) As String
        Dim Pasco_County_Face_Value(Pasco_County_record_length) As String
        Dim Pasco_County_Tax_Year(Pasco_County_record_length) As String
        Dim Pasco_County_Parcel_Number(Pasco_County_record_length) As String
        Dim Pasco_County_Owner_Name_1(Pasco_County_record_length) As String
        Dim Pasco_County_Owner_Name_2(Pasco_County_record_length) As String
        Dim Pasco_County_Owner_Address_1(Pasco_County_record_length) As String
        Dim Pasco_County_Owner_Address_2(Pasco_County_record_length) As String
        Dim Pasco_County_Owner_Address_3(Pasco_County_record_length) As String
        Dim Pasco_County_Interest_Rate(Pasco_County_record_length) As String
        Dim Pasco_County_Redemption_Date(Pasco_County_record_length) As String
        Dim Pasco_County_Redemption_Amount(Pasco_County_record_length) As String
        Dim Pasco_County_Tax_Area(Pasco_County_record_length) As String
        'Reading the Pasco county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Pasco\Pasco County.csv"
        Dim count As Integer = 0
        Dim fileRows(), fileFields() As String

        If File.Exists(fileIn) Then
            Dim fileStream As StreamReader = File.OpenText(fileIn)
            fileRows = fileStream.ReadToEnd().Split(Environment.NewLine)
            'if i=0 csv reading with header, i=1 skip the header
            For i As Integer = 0 To fileRows.Length - 1
                'fileFields = fileRows(i).Split(",")

                Dim pattern As String = "[\t,](?=(?:[^\""]|\""[^\""]*\"")*$)" 'regular expression to parse csv 
                fileFields = Regex.Split(fileRows(i), pattern)


                If count < fileRows.Length - 1 Then
                    Pasco_County_Certificate_Number(count) = fileFields(0).Replace("""", "")
                    Pasco_County_Face_Value(count) = fileFields(1).Replace("""", "")
                    Pasco_County_Tax_Year(count) = fileFields(2).Replace("""", "")
                    Pasco_County_Parcel_Number(count) = fileFields(3).Replace("""", "")
                    Pasco_County_Owner_Name_1(count) = fileFields(4).Replace("""", "")
                    Pasco_County_Owner_Name_2(count) = fileFields(5).Replace("""", "")
                    Pasco_County_Owner_Address_1(count) = fileFields(6).Replace("""", "")
                    Pasco_County_Owner_Address_2(count) = fileFields(7).Replace("""", "")
                    Pasco_County_Owner_Address_3(count) = fileFields(8).Replace("""", "")
                    Pasco_County_Interest_Rate(count) = fileFields(9).Replace("""", "")
                    Pasco_County_Redemption_Date(count) = fileFields(10).Replace("""", "")
                    Pasco_County_Redemption_Amount(count) = fileFields(11).Replace("""", "")
                    Pasco_County_Tax_Area(count) = fileFields(12).Replace("""", "")
                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\Pasco_ufc_formatted.txt")

        Dim UFC_Parcel_County As String
        Dim UFC_Parcel_ID As String
        Dim UFC_Parcel_Status As String
        Dim UFC_Parcel_Tax_District As String
        Dim UFC_Parcel_Just_Value As Integer
        Dim UFC_Parcel_Taxable_Value As Integer
        Dim UFC_Parcel_Street_Address_1 As String
        Dim UFC_Parcel_Street_Address_2 As String
        Dim UFC_Parcel_Street_Address_3 As String
        Dim UFC_Parcel_Street_Address_4 As String
        Dim UFC_Parcel_Street_Address_5 As String
        Dim UFC_Parcel_Street_Address_6 As String
        Dim UFC_Parcel_Street_Address As String = ""
        Dim UFC_Parcel_City As String = ""
        Dim UFC_Parcel_State As String = ""
        Dim UFC_Parcel_ZIP_Code As String = ""
        Dim UFC_Parcel_ZIP_Code4 As String = ""
        Dim UFC_Parcel_Owner_First_Name As String
        Dim UFC_Parcel_Owner_Last_Name As String
        Dim UFC_Parcel_Owner_Street_Address_1 As String
        Dim UFC_Parcel_Owner_Street_Address_2 As String
        Dim UFC_Parcel_Owner_Street_Address_3 As String
        Dim UFC_Parcel_Owner_Street_Address_4 As String
        Dim UFC_Parcel_Owner_Street_Address_5 As String
        Dim UFC_Parcel_Owner_Street_Address_6 As String
        Dim UFC_Parcel_Owner_street_Address As String
        Dim UFC_Parcel_Owner_City As String
        Dim UFC_Parcel_Owner_State As String
        Dim UFC_Parcel_Owner_ZIP_Code As String
        Dim UFC_Parcel_Owner_ZIP_Code4 As String
        Dim UFC_Parcel_Owner_Country As String
        Dim UFC_Parcel_Num_of_Exemptions As Integer
        Dim UFC_Parcel_Total_Ex_Amount As Integer
        Dim UFC_Parcel_Exemption_Type_1 As String
        Dim UFC_Parcel_Exemption_Type_2 As String
        Dim UFC_Parcel_Exemption_Type_3 As String
        Dim UFC_Parcel_Exemption_Type_4 As String
        Dim UFC_Parcel_Exemption_Type_5 As String
        Dim UFC_Parcel_Exemption_Type_6 As String
        Dim UFC_Parcel_Exemption_Amount_1 As Integer
        Dim UFC_Parcel_Exemption_Amount_2 As Integer
        Dim UFC_Parcel_Exemption_Amount_3 As Integer
        Dim UFC_Parcel_Exemption_Amount_4 As Integer
        Dim UFC_Parcel_Exemption_Amount_5 As Integer
        Dim UFC_Parcel_Exemption_Amount_6 As Integer
        Dim UFC_Parcel_Legal_Description As String
        Dim UFC_TC_Number As String
        Dim UFC_TC_Status As String
        Dim UFC_TC_Issue_Year As String
        Dim UFC_TC_Tax_Year As String
        Dim UFC_TC_Face_Value As Integer
        Dim UFC_TC_Sale_Date As Date
        Dim UFC_TC_Redemption_Date As Date
        Dim UFC_TC_Interest_Rate As Decimal
        Dim UFC_TC_Redemption_Amount As Integer
        Dim UFC_TC_Days_Unpaid As Integer
        'writing variables in UFC
        writeCsv.WriteLine("ParcelCounty~ParcelID~ParcelStatus~ParcelTaxDistrict~ParcelJustValue~ParcelTaxableValue~ParcelStreetAddress1~ParcelStreetAddress2~ParcelStreetAddress3~ParcelStreetAddress4~ParcelStreetAddress5~ParcelStreetAddress6~ParcelStreetAddress~ParcelCity~ParcelState~ParcelZIPCode~ParcelZIPCode+4~ParcelOwnerFirstName~ParcelOwnerLastName~ParcelOwnerStreetAddress1~ParcelOwnerStreetAddress2~ParcelOwnerStreetAddress3~ParcelOwnerStreetAddress4~ParcelOwnerStreetAddress5~ParcelOwnerStreetAddress6~ParcelOwnerStreetAddress~ParcelOwnerCity~ParcelOwnerState~ParcelOwnerZIPCode~ParcelOwnerZIPCode+4~ParcelOwnerCountry~ParcelNum.ofExemptions~ParcelTotalEx.Amount~ParcelExemptionType1~ParcelExemptionType2~ParcelExemptionType3~ParcelExemptionType4~ParcelExemptionType5~ParcelExemptionType6~ParcelExemptionAmount1~ParcelExemptionAmount2~ParcelExemptionAmount3~ParcelExemptionAmount4~ParcelExemptionAmount5~ParcelExemptionAmount6~ParcelLegalDescription~TCNumber~TCStatus~TCIssueYear~TCTaxYear~TCFaceValue~TCSaleDate~TCRedemptionDate~TCInterestRate~TCRedemptionAmount~TCDaysUnpaid~ZP4AddressStatus~Google&BingAddressStatus")
        Dim correctedAddress As New Hashtable
        Dim zp4ParseStatus As String = ""
        Dim googleParseStatus As String = ""
        Dim oldParcelId As String = ""
        For i As Integer = 1 To Pasco_County_record_length - 1

            UFC_Parcel_County = "Pasco"
            UFC_Parcel_ID = Pasco_County_Parcel_Number(i).Trim
            Console.WriteLine(UFC_Parcel_ID)
            UFC_Parcel_Status = ""
            UFC_Parcel_Tax_District = Pasco_County_Tax_Area(i).Trim
            UFC_Parcel_Just_Value = -1
            UFC_Parcel_Taxable_Value = -1
            UFC_Parcel_Street_Address_1 = ""
            UFC_Parcel_Street_Address_2 = ""
            UFC_Parcel_Street_Address_3 = ""
            UFC_Parcel_Street_Address_4 = ""
            UFC_Parcel_Street_Address_5 = ""
            UFC_Parcel_Street_Address_6 = ""
            UFC_Parcel_Street_Address = ""
            zp4ParseStatus = "ZP4:Invalid Address"
            googleParseStatus = "Google:Invalid Address"
            UFC_Parcel_City = ""
            UFC_Parcel_State = ""
            UFC_Parcel_ZIP_Code = ""
            UFC_Parcel_ZIP_Code4 = ""
            UFC_Parcel_Owner_First_Name = Pasco_County_Owner_Name_1(i).Trim & " " & Pasco_County_Owner_Name_2(i).Trim
            UFC_Parcel_Owner_Last_Name = ""
            UFC_Parcel_Owner_Street_Address_1 = Pasco_County_Owner_Address_1(i).Trim
            UFC_Parcel_Owner_Street_Address_2 = Pasco_County_Owner_Address_2(i).Trim
            UFC_Parcel_Owner_Street_Address_3 = Pasco_County_Owner_Address_3(i).Trim
            UFC_Parcel_Owner_Street_Address_4 = ""
            UFC_Parcel_Owner_Street_Address_5 = ""
            UFC_Parcel_Owner_Street_Address_6 = ""
            UFC_Parcel_Owner_street_Address = ""
            UFC_Parcel_Owner_City = ""
            UFC_Parcel_Owner_State = ""
            UFC_Parcel_Owner_ZIP_Code = ""
            UFC_Parcel_Owner_ZIP_Code4 = ""
            UFC_Parcel_Owner_Country = ""
            UFC_Parcel_Num_of_Exemptions = -1
            UFC_Parcel_Total_Ex_Amount = -1
            UFC_Parcel_Exemption_Type_1 = ""
            UFC_Parcel_Exemption_Type_2 = ""
            UFC_Parcel_Exemption_Type_3 = ""
            UFC_Parcel_Exemption_Type_4 = ""
            UFC_Parcel_Exemption_Type_5 = ""
            UFC_Parcel_Exemption_Type_6 = ""
            UFC_Parcel_Exemption_Amount_1 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_2 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_3 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_4 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_5 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_6 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Legal_Description = ""
            UFC_TC_Number = Pasco_County_Certificate_Number(i).Trim
            UFC_TC_Status = ""

            Select Case UFC_TC_Status
                Case "Paid in full"
                    UFC_TC_Status = "Paid"
                Case "DEFRRD"
                    UFC_TC_Status = "Deferred"
                Case "In House Cancellation"
                    UFC_TC_Status = "Canceled"
                Case "P", "PAID"
                    UFC_TC_Status = "Paid"
                Case ""
                    UFC_TC_Status = ""
                Case Else
                    UFC_TC_Status = UFC_TC_Status
            End Select

            UFC_TC_Issue_Year = "" ' need to strip years only
            UFC_TC_Tax_Year = Pasco_County_Tax_Year(i).Trim
            UFC_TC_Face_Value = Pasco_County_Face_Value(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = ""
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse("")
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Pasco_County_Redemption_Date(i).Trim
            tc_remDate = tc_remDate.PadLeft(6, "0")
            Dim mm As String
            Dim dd As String
            Dim yy As String
            mm = tc_remDate.Substring(0, tc_remDate.Length - 4)
            dd = tc_remDate.Substring(tc_remDate.Length - 4, tc_remDate.Length - 4)
            yy = tc_remDate.Substring(tc_remDate.Length - 2)
            yy = "20" & "" & yy
            tc_remDate = mm & "/" & dd & "/" & yy
            If tc_remDate = "/  /" Or tc_remDate = "00/00/2000" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(tc_remDate.Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If

            'Interest rate
            UFC_TC_Interest_Rate = Pasco_County_Interest_Rate(i).Trim

            'Redem amount
            If Pasco_County_Redemption_Amount(i).Trim = "" Then
                UFC_TC_Redemption_Amount = -1
            Else
                UFC_TC_Redemption_Amount = Pasco_County_Redemption_Amount(i).Trim
            End If

            'Calculate unpaid days
            If UFC_TC_Sale_Date = Date.MinValue Or UFC_TC_Redemption_Date = Date.MinValue Then
                UFC_TC_Days_Unpaid = -1
            Else
                UFC_TC_Days_Unpaid = DateDiff(DateInterval.Day, UFC_TC_Sale_Date, UFC_TC_Redemption_Date)
            End If

            'Bad data flag


            ' Write into CSV
            writeCsv.WriteLine(UFC_Parcel_County.Trim() & "~" & UFC_Parcel_ID & "~" &
           UFC_Parcel_Status & "~" &
           UFC_Parcel_Tax_District & "~" &
           UFC_Parcel_Just_Value & "~" &
           UFC_Parcel_Taxable_Value & "~" &
           UFC_Parcel_Street_Address_1 & "~" &
           UFC_Parcel_Street_Address_2 & "~" &
           UFC_Parcel_Street_Address_3 & "~" &
           UFC_Parcel_Street_Address_4 & "~" &
           UFC_Parcel_Street_Address_5 & "~" &
           UFC_Parcel_Street_Address_6 & "~" &
           UFC_Parcel_Street_Address & "~" &
           UFC_Parcel_City & "~" &
           UFC_Parcel_State & "~" &
           UFC_Parcel_ZIP_Code & "~" &
           UFC_Parcel_ZIP_Code4 & "~" &
           UFC_Parcel_Owner_First_Name & "~" &
           UFC_Parcel_Owner_Last_Name & "~" &
           UFC_Parcel_Owner_Street_Address_1 & "~" &
           UFC_Parcel_Owner_Street_Address_2 & "~" &
           UFC_Parcel_Owner_Street_Address_3 & "~" &
           UFC_Parcel_Owner_Street_Address_4 & "~" &
           UFC_Parcel_Owner_Street_Address_5 & "~" &
           UFC_Parcel_Owner_Street_Address_6 & "~" &
           UFC_Parcel_Owner_street_Address & "~" &
           UFC_Parcel_Owner_City & "~" &
           UFC_Parcel_Owner_State & "~" &
           UFC_Parcel_Owner_ZIP_Code & "~" &
           UFC_Parcel_Owner_ZIP_Code4 & "~" &
           UFC_Parcel_Owner_Country & "~" &
           UFC_Parcel_Num_of_Exemptions & "~" &
           UFC_Parcel_Total_Ex_Amount & "~" &
           UFC_Parcel_Exemption_Type_1 & "~" &
           UFC_Parcel_Exemption_Type_2 & "~" &
           UFC_Parcel_Exemption_Type_3 & "~" &
           UFC_Parcel_Exemption_Type_4 & "~" &
           UFC_Parcel_Exemption_Type_5 & "~" &
           UFC_Parcel_Exemption_Type_6 & "~" &
           UFC_Parcel_Exemption_Amount_1 & "~" &
           UFC_Parcel_Exemption_Amount_2 & "~" &
           UFC_Parcel_Exemption_Amount_3 & "~" &
           UFC_Parcel_Exemption_Amount_4 & "~" &
           UFC_Parcel_Exemption_Amount_5 & "~" &
           UFC_Parcel_Exemption_Amount_6 & "~" &
           UFC_Parcel_Legal_Description & "~" &
           UFC_TC_Number & "~" &
           UFC_TC_Status & "~" &
           UFC_TC_Issue_Year & "~" &
           UFC_TC_Tax_Year & "~" &
           UFC_TC_Face_Value & "~" &
           UFC_TC_Sale_Date & "~" &
           UFC_TC_Redemption_Date & "~" &
           UFC_TC_Interest_Rate & "~" &
           UFC_TC_Redemption_Amount & "~" &
           UFC_TC_Days_Unpaid & "~" &
           zp4ParseStatus & "~" &
           googleParseStatus)
        Next



        writeCsv.Close()
        Console.WriteLine("Written into file completed....")

    End Sub
End Module
