Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Text
Imports System.Net
Imports System.Xml
Module Columbia
    Sub ParseColumbiaCounty()
        Dim Columbia_record_length As Integer = 19853
        'variables
        Dim Columbia_ACCT(Columbia_record_length) As String
        Dim Columbia_TAXM_ST_NO(Columbia_record_length) As String
        Dim Columbia_TAXM_ST_NAME(Columbia_record_length) As String
        Dim Columbia_TAXM_ST_CITY(Columbia_record_length) As String
        Dim Columbia_OWNR_ADDR1(Columbia_record_length) As String
        Dim Columbia_OWNR_ADDR2(Columbia_record_length) As String
        Dim Columbia_OWNR_ADDR3(Columbia_record_length) As String
        Dim Columbia_OWNR_ADDR4(Columbia_record_length) As String
        Dim Columbia_OWNR_ADDR5(Columbia_record_length) As String
        Dim Columbia_OWNR_ADDR6(Columbia_record_length) As String
        Dim Columbia_OWNR_ADDR7(Columbia_record_length) As String
        Dim Columbia_OWNR_ZIP_CODE(Columbia_record_length) As String
        Dim Columbia_EXEM_CODE(Columbia_record_length) As String
        Dim Columbia_VALUE(Columbia_record_length) As String
        Dim Columbia_LEGAL01(Columbia_record_length) As String
        Dim Columbia_LEGAL02(Columbia_record_length) As String
        Dim Columbia_LEGAL03(Columbia_record_length) As String
        Dim Columbia_LEGAL04(Columbia_record_length) As String
        Dim Columbia_CERT_NUMBER(Columbia_record_length) As String
        Dim Columbia_CERT_YEAR(Columbia_record_length) As String
        Dim Columbia_TAX_YEAR(Columbia_record_length) As String
        Dim Columbia_FACE_AMT(Columbia_record_length) As String
        Dim Columbia_CERT_DATE_SOLD(Columbia_record_length) As String
        Dim Columbia_DATE_REDEEMED(Columbia_record_length) As String
        Dim Columbia_BID_PERCENT(Columbia_record_length) As String
        'Reading the Columbia csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Columbia\Columbia County.csv"
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
                    Columbia_ACCT(count) = fileFields(0).Replace("""", "")
                    Columbia_TAXM_ST_NO(count) = fileFields(1).Replace("""", "")
                    Columbia_TAXM_ST_NAME(count) = fileFields(2).Replace("""", "")
                    Columbia_TAXM_ST_CITY(count) = fileFields(3).Replace("""", "")
                    Columbia_OWNR_ADDR1(count) = fileFields(4).Replace("""", "")
                    Columbia_OWNR_ADDR2(count) = fileFields(5).Replace("""", "")
                    Columbia_OWNR_ADDR3(count) = fileFields(6).Replace("""", "")
                    Columbia_OWNR_ADDR4(count) = fileFields(7).Replace("""", "")
                    Columbia_OWNR_ADDR5(count) = fileFields(8).Replace("""", "")
                    Columbia_OWNR_ADDR6(count) = fileFields(9).Replace("""", "")
                    Columbia_OWNR_ADDR7(count) = fileFields(10).Replace("""", "")
                    Columbia_OWNR_ZIP_CODE(count) = fileFields(11).Replace("""", "")
                    Columbia_EXEM_CODE(count) = fileFields(12).Replace("""", "")
                    Columbia_VALUE(count) = fileFields(13).Replace("""", "")
                    Columbia_LEGAL01(count) = fileFields(14).Replace("""", "")
                    Columbia_LEGAL02(count) = fileFields(15).Replace("""", "")
                    Columbia_LEGAL03(count) = fileFields(16).Replace("""", "")
                    Columbia_LEGAL04(count) = fileFields(17).Replace("""", "")
                    Columbia_CERT_NUMBER(count) = fileFields(18).Replace("""", "")
                    Columbia_CERT_YEAR(count) = fileFields(19).Replace("""", "")
                    Columbia_TAX_YEAR(count) = fileFields(20).Replace("""", "")
                    Columbia_FACE_AMT(count) = fileFields(21).Replace("""", "")
                    Columbia_CERT_DATE_SOLD(count) = fileFields(22).Replace("""", "")
                    Columbia_DATE_REDEEMED(count) = fileFields(23).Replace("""", "")
                    Columbia_BID_PERCENT(count) = fileFields(24).Replace("""", "")
                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        Console.WriteLine(Columbia_ACCT(1))
        Console.WriteLine(Columbia_CERT_DATE_SOLD(19853))
        Console.WriteLine(Columbia_DATE_REDEEMED(19853))
        Console.WriteLine(Columbia_VALUE(19853))

        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\Columbia_ufc_formatted.txt")
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
        Dim tempAddress As String
        Dim oldParcelId As String = ""
        For i As Integer = 1 To Columbia_record_length - 1
            UFC_Parcel_County = "Columbia"
            UFC_Parcel_ID = Columbia_ACCT(i).Trim
            Console.WriteLine(UFC_Parcel_ID)
            UFC_Parcel_Status = ""
            UFC_Parcel_Tax_District = ""
            UFC_Parcel_Just_Value = Columbia_VALUE(i).Trim
            UFC_Parcel_Taxable_Value = -1
            UFC_Parcel_Street_Address_1 = Columbia_TAXM_ST_NO(i).Trim
            UFC_Parcel_Street_Address_2 = Columbia_TAXM_ST_NAME(i).Trim
            UFC_Parcel_Street_Address_3 = ""
            UFC_Parcel_Street_Address_4 = ""
            UFC_Parcel_Street_Address_5 = ""
            UFC_Parcel_Street_Address_6 = ""

            If (oldParcelId = UFC_Parcel_ID) Then
                UFC_Parcel_City = UFC_Parcel_City
                UFC_Parcel_State = UFC_Parcel_State
                UFC_Parcel_ZIP_Code = UFC_Parcel_ZIP_Code
                UFC_Parcel_Street_Address = UFC_Parcel_Street_Address
                UFC_Parcel_ZIP_Code4 = UFC_Parcel_ZIP_Code4
                googleParseStatus = googleParseStatus
                zp4ParseStatus = zp4ParseStatus
            Else
                oldParcelId = UFC_Parcel_ID
                tempAddress = Columbia_TAXM_ST_NO(i).Trim & " " & Columbia_TAXM_ST_NAME(i).Trim & " " & Columbia_TAXM_ST_CITY(i).Trim
                correctedAddress = GetCorrectedAddress(tempAddress)
                UFC_Parcel_City = correctedAddress("city")
                UFC_Parcel_State = correctedAddress("state")
                UFC_Parcel_ZIP_Code = correctedAddress("zip")
                UFC_Parcel_ZIP_Code4 = correctedAddress("zip4")
                UFC_Parcel_Street_Address = correctedAddress("street")
                googleParseStatus = correctedAddress("googlebingstatus")
                zp4ParseStatus = correctedAddress("zp4status")
            End If
            UFC_Parcel_Owner_First_Name = ""
            UFC_Parcel_Owner_Last_Name = ""
            UFC_Parcel_Owner_Street_Address_1 = Columbia_OWNR_ADDR1(i).Trim & " " & Columbia_OWNR_ADDR2(i).Trim & " " & Columbia_OWNR_ADDR3(i).Trim & " " & Columbia_OWNR_ADDR4(i).Trim & " " & Columbia_OWNR_ADDR5(i).Trim & " " & Columbia_OWNR_ADDR6(i).Trim & " " & Columbia_OWNR_ADDR7(i).Trim
            UFC_Parcel_Owner_Street_Address_2 = ""
            UFC_Parcel_Owner_Street_Address_3 = ""
            UFC_Parcel_Owner_Street_Address_4 = ""
            UFC_Parcel_Owner_Street_Address_5 = ""
            UFC_Parcel_Owner_Street_Address_6 = ""
            UFC_Parcel_Owner_street_Address = ""
            UFC_Parcel_Owner_City = ""
            UFC_Parcel_Owner_State = ""
            UFC_Parcel_Owner_ZIP_Code = Columbia_OWNR_ZIP_CODE(i).Trim
            UFC_Parcel_Owner_ZIP_Code4 = ""
            UFC_Parcel_Owner_Country = ""
            UFC_Parcel_Num_of_Exemptions = -1
            UFC_Parcel_Total_Ex_Amount = -1
            UFC_Parcel_Exemption_Type_1 = Columbia_EXEM_CODE(i).Trim
            UFC_Parcel_Exemption_Type_2 = ""
            UFC_Parcel_Exemption_Type_3 = ""
            UFC_Parcel_Exemption_Type_4 = ""
            UFC_Parcel_Exemption_Type_5 = ""
            UFC_Parcel_Exemption_Type_6 = ""
            UFC_Parcel_Exemption_Amount_1 = -1
            UFC_Parcel_Exemption_Amount_2 = -1
            UFC_Parcel_Exemption_Amount_3 = -1
            UFC_Parcel_Exemption_Amount_4 = -1
            UFC_Parcel_Exemption_Amount_5 = -1
            UFC_Parcel_Exemption_Amount_6 = -1
            UFC_Parcel_Legal_Description = Columbia_LEGAL01(i).Trim & " " & Columbia_LEGAL02(i).Trim & " " & Columbia_LEGAL03(i).Trim & " " & Columbia_LEGAL04(i).Trim
            UFC_TC_Number = Columbia_CERT_NUMBER(i).Trim
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
            UFC_TC_Issue_Year = Columbia_CERT_YEAR(i).Trim ' need to strip years only
            UFC_TC_Tax_Year = Columbia_TAX_YEAR(i).Trim
            UFC_TC_Face_Value = Columbia_FACE_AMT(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = Columbia_CERT_DATE_SOLD(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(Columbia_CERT_DATE_SOLD(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Columbia_DATE_REDEEMED(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(Columbia_DATE_REDEEMED(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If

            'Interest rate


            UFC_TC_Interest_Rate = Columbia_BID_PERCENT(i).Trim

            If Columbia_BID_PERCENT(i).Trim = "" Then
                UFC_TC_Redemption_Amount = -1
            Else
                UFC_TC_Redemption_Amount = Columbia_BID_PERCENT(i).Trim
            End If

            'Calculate unpaid days
            If UFC_TC_Sale_Date = Date.MinValue Or UFC_TC_Redemption_Date = Date.MinValue Then
                UFC_TC_Days_Unpaid = -1
            Else
                UFC_TC_Days_Unpaid = DateDiff(DateInterval.Day, UFC_TC_Sale_Date, UFC_TC_Redemption_Date)
            End If


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
