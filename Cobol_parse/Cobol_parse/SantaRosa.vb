Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Text
Imports System.Net
Imports System.Xml
Module SantaRosa
    Sub ParseSantaRosaCounty()
        Dim SantaRosa_record_length As Integer = 42598
        'variables
        Dim SantaRosa_ACCT(SantaRosa_record_length) As String
        Dim SantaRosa_VALUE(SantaRosa_record_length) As String
        Dim SantaRosa_TAXM_ST_NO(SantaRosa_record_length) As String
        Dim SantaRosa_TAXM_ST_NAME(SantaRosa_record_length) As String
        Dim SantaRosa_OWNR_ADDR1(SantaRosa_record_length) As String
        Dim SantaRosa_OWNR_ADDR2(SantaRosa_record_length) As String
        Dim SantaRosa_OWNR_ADDR3(SantaRosa_record_length) As String
        Dim SantaRosa_OWNR_ZIP_CODE(SantaRosa_record_length) As String
        Dim SantaRosa_EXEM_CODE(SantaRosa_record_length) As String
        Dim SantaRosa_LEGAL01(SantaRosa_record_length) As String
        Dim SantaRosa_LEGAL02(SantaRosa_record_length) As String
        Dim SantaRosa_CERT_NUMBER(SantaRosa_record_length) As String
        Dim SantaRosa_CERT_YEAR(SantaRosa_record_length) As String
        Dim SantaRosa_TAX_YEAR(SantaRosa_record_length) As String
        Dim SantaRosa_FACE_AMT(SantaRosa_record_length) As String
        Dim SantaRosa_CERT_DATE_SOLD(SantaRosa_record_length) As String
        Dim SantaRosa_DATE_REDEEMED(SantaRosa_record_length) As String
        Dim SantaRosa_BID_PERCENT(SantaRosa_record_length) As String
        'Reading the Santa county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Santa Rosa\SantaRosa.csv"
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
                    SantaRosa_ACCT(count) = fileFields(0).Replace("""", "")
                    'Console.WriteLine(SantaRosa_ACCT(count))
                    SantaRosa_VALUE(count) = fileFields(1).Replace("""", "")
                    SantaRosa_TAXM_ST_NO(count) = fileFields(2).Replace("""", "")
                    SantaRosa_TAXM_ST_NAME(count) = fileFields(3).Replace("""", "")
                    SantaRosa_OWNR_ADDR1(count) = fileFields(4).Replace("""", "")
                    SantaRosa_OWNR_ADDR2(count) = fileFields(5).Replace("""", "")
                    SantaRosa_OWNR_ADDR3(count) = fileFields(6).Replace("""", "")
                    SantaRosa_OWNR_ZIP_CODE(count) = fileFields(7).Replace("""", "")
                    SantaRosa_EXEM_CODE(count) = fileFields(8).Replace("""", "")
                    SantaRosa_LEGAL01(count) = fileFields(9).Replace("""", "")
                    SantaRosa_LEGAL02(count) = fileFields(10).Replace("""", "")
                    SantaRosa_CERT_NUMBER(count) = fileFields(11).Replace("""", "")
                    SantaRosa_CERT_YEAR(count) = fileFields(12).Replace("""", "")
                    SantaRosa_TAX_YEAR(count) = fileFields(13).Replace("""", "")
                    SantaRosa_FACE_AMT(count) = fileFields(14).Replace("""", "")
                    SantaRosa_CERT_DATE_SOLD(count) = fileFields(15).Replace("""", "")
                    SantaRosa_DATE_REDEEMED(count) = fileFields(16).Replace("""", "")
                    SantaRosa_BID_PERCENT(count) = fileFields(17).Replace("""", "")
                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        Console.WriteLine(SantaRosa_ACCT(1))
        Console.WriteLine(SantaRosa_CERT_DATE_SOLD(1))
        Console.WriteLine(SantaRosa_ACCT(42598))
        Console.WriteLine(SantaRosa_CERT_DATE_SOLD(42589))

        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\SantaRosa_ufc_formatted_csv.txt")
        Dim UFC_Parcel_County As String
        Dim UFC_Parcel_ID As String = ""
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
        Dim UFC_Parcel_Owner_Street_Address As String
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

        For i As Integer = 9342 To SantaRosa_record_length - 1

            UFC_Parcel_County = "SantaRosa"
            Try
                UFC_Parcel_ID = SantaRosa_ACCT(i).Trim
                Console.WriteLine(UFC_Parcel_ID)
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
            End Try

            UFC_Parcel_Status = ""
            Select Case UFC_Parcel_Status
                Case "Paid in full"
                    UFC_Parcel_Status = "Paid"
                Case ""
                    UFC_Parcel_Status = ""
                Case Else
                    UFC_Parcel_Status = UFC_Parcel_Status
            End Select
            UFC_Parcel_Tax_District = ""
            UFC_Parcel_Just_Value = SantaRosa_VALUE(i).Trim
            UFC_Parcel_Taxable_Value = -1
            UFC_Parcel_Street_Address_1 = SantaRosa_TAXM_ST_NO(i).Trim & " " & SantaRosa_TAXM_ST_NAME(i).Trim
            UFC_Parcel_Street_Address_2 = ""
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
                tempAddress = SantaRosa_TAXM_ST_NO(i).Trim & " " & SantaRosa_TAXM_ST_NAME(i).Trim
                correctedAddress = GetCorrectedAddress(tempAddress)
                UFC_Parcel_City = correctedAddress("city")
                UFC_Parcel_State = correctedAddress("state")
                UFC_Parcel_ZIP_Code = correctedAddress("zip")
                UFC_Parcel_ZIP_Code4 = correctedAddress("zip4")
                UFC_Parcel_Street_Address = correctedAddress("street")
                googleParseStatus = correctedAddress("googlebingstatus")
                zp4ParseStatus = correctedAddress("zp4status")
            End If

            UFC_Parcel_Owner_First_Name = SantaRosa_OWNR_ADDR1(i).Trim
            UFC_Parcel_Owner_Last_Name = ""
            UFC_Parcel_Owner_Street_Address_1 = SantaRosa_OWNR_ADDR2(i).Trim
            UFC_Parcel_Owner_Street_Address_2 = SantaRosa_OWNR_ADDR3(i).Trim
            UFC_Parcel_Owner_Street_Address_3 = ""
            UFC_Parcel_Owner_Street_Address_4 = ""
            UFC_Parcel_Owner_Street_Address_5 = ""
            UFC_Parcel_Owner_Street_Address_6 = ""
            UFC_Parcel_Owner_Street_Address = ""
            UFC_Parcel_Owner_City = ""
            UFC_Parcel_Owner_State = ""
            UFC_Parcel_Owner_ZIP_Code = SantaRosa_OWNR_ZIP_CODE(i).Trim
            UFC_Parcel_Owner_ZIP_Code4 = ""
            UFC_Parcel_Owner_Country = ""
            UFC_Parcel_Num_of_Exemptions = -1
            UFC_Parcel_Total_Ex_Amount = -1
            UFC_Parcel_Exemption_Type_1 = SantaRosa_EXEM_CODE(i).Trim
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
            UFC_Parcel_Legal_Description = SantaRosa_LEGAL01(i).Trim & " " & SantaRosa_LEGAL02(i).Trim
            UFC_TC_Number = SantaRosa_CERT_NUMBER(i).Trim
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
            UFC_TC_Issue_Year = SantaRosa_CERT_YEAR(i).Trim ' need to strip years only
            UFC_TC_Tax_Year = ""
            UFC_TC_Face_Value = SantaRosa_FACE_AMT(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = SantaRosa_CERT_DATE_SOLD(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(SantaRosa_CERT_DATE_SOLD(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = SantaRosa_DATE_REDEEMED(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(SantaRosa_DATE_REDEEMED(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If


            UFC_TC_Interest_Rate = SantaRosa_BID_PERCENT(i).Trim
            UFC_TC_Redemption_Amount = -1
            'Calculate unpaid days
            If UFC_TC_Sale_Date = Date.MinValue Or UFC_TC_Redemption_Date = Date.MinValue Then
                UFC_TC_Days_Unpaid = -1
            Else
                UFC_TC_Days_Unpaid = DateDiff(DateInterval.Day, UFC_TC_Sale_Date, UFC_TC_Redemption_Date)
            End If

            'Write into CSV
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
           UFC_Parcel_Owner_Street_Address & "~" &
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
