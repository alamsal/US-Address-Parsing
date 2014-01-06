Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Text
Imports System.Net
Imports System.Xml
Module Hillsborough
    Sub ParseHillsboroughCounty()

        Dim Hillborough_record_length As Integer = 342186
        'variables
        Dim Hillsborough_County_Folio_Number(Hillborough_record_length) As String
        Dim Hillsborough_County_Tax_Year(Hillborough_record_length) As String
        Dim Hillsborough_County_App_Code(Hillborough_record_length) As String
        Dim Hillsborough_County_Mail_to_Name(Hillborough_record_length) As String
        Dim Hillsborough_County_Mail_to_Address1(Hillborough_record_length) As String
        Dim Hillsborough_County_Mail_to_Address2(Hillborough_record_length) As String
        Dim Hillsborough_County_Mail_to_Address3(Hillborough_record_length) As String
        Dim Hillsborough_County_Mail_to_Address4(Hillborough_record_length) As String
        Dim Hillsborough_County_Mail_to_City(Hillborough_record_length) As String
        Dim Hillsborough_County_Mail_to_State(Hillborough_record_length) As String
        Dim Hillsborough_County_Mail_to_Zip(Hillborough_record_length) As String
        Dim Hillsborough_County_Location_Street_Number(Hillborough_record_length) As String
        Dim Hillsborough_County_Location_Direction(Hillborough_record_length) As String
        Dim Hillsborough_County_Location_Street_Name(Hillborough_record_length) As String
        Dim Hillsborough_County_Location_Designator(Hillborough_record_length) As String
        Dim Hillsborough_County_Location_Suite(Hillborough_record_length) As String
        Dim Hillsborough_County_Location_City(Hillborough_record_length) As String
        Dim Hillsborough_County_Location_Zip(Hillborough_record_length) As String
        Dim Hillsborough_County_District_Code(Hillborough_record_length) As String
        Dim Hillsborough_County_Land_Value(Hillborough_record_length) As String
        Dim Hillsborough_County_Just_Value(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Code_1(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Amount_1(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Code_2(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Amount_2(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Code_3(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Amount_3(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Code_4(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Amount_4(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Code_5(Hillborough_record_length) As String
        Dim Hillsborough_County_Exemption_Amount_5(Hillborough_record_length) As String
        Dim Hillsborough_County_AR_Receipt_Date(Hillborough_record_length) As String
        Dim Hillsborough_County_Certificate_Number(Hillborough_record_length) As String
        Dim Hillsborough_County_Face_Amount(Hillborough_record_length) As String
        Dim Hillsborough_County_Sale_Date(Hillborough_record_length) As String
        Dim Hillsborough_County_Interest_Rate(Hillborough_record_length) As String
        Dim Hillsborough_County_Legal_Description(Hillborough_record_length) As String
        'Reading the Hills county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Hillsborough\Hillsborough County.csv"
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
                    Try
                        Hillsborough_County_Folio_Number(count) = fileFields(0).Replace("""", "")
                        'Console.WriteLine(Hillsborough_County_Folio_Number(count))
                        Hillsborough_County_Tax_Year(count) = fileFields(1).Replace("""", "")
                        Hillsborough_County_App_Code(count) = fileFields(2).Replace("""", "")
                        Hillsborough_County_Mail_to_Name(count) = fileFields(3).Replace("""", "")
                        Hillsborough_County_Mail_to_Address1(count) = fileFields(4).Replace("""", "")
                        Hillsborough_County_Mail_to_Address2(count) = fileFields(5).Replace("""", "")
                        Hillsborough_County_Mail_to_Address3(count) = fileFields(6).Replace("""", "")
                        Hillsborough_County_Mail_to_Address4(count) = fileFields(7).Replace("""", "")
                        Hillsborough_County_Mail_to_City(count) = fileFields(8).Replace("""", "")
                        Hillsborough_County_Mail_to_State(count) = fileFields(9).Replace("""", "")
                        Hillsborough_County_Mail_to_Zip(count) = fileFields(10).Replace("""", "")
                        Hillsborough_County_Location_Street_Number(count) = fileFields(11).Replace("""", "")
                        Hillsborough_County_Location_Direction(count) = fileFields(12).Replace("""", "")
                        Hillsborough_County_Location_Street_Name(count) = fileFields(13).Replace("""", "")
                        Hillsborough_County_Location_Designator(count) = fileFields(14).Replace("""", "")
                        Hillsborough_County_Location_Suite(count) = fileFields(15).Replace("""", "")
                        Hillsborough_County_Location_City(count) = fileFields(16).Replace("""", "")
                        Hillsborough_County_Location_Zip(count) = fileFields(17).Replace("""", "")
                        Hillsborough_County_District_Code(count) = fileFields(18).Replace("""", "")
                        Hillsborough_County_Land_Value(count) = fileFields(19).Replace("""", "")
                        Hillsborough_County_Just_Value(count) = fileFields(20).Replace("""", "")
                        Hillsborough_County_Exemption_Code_1(count) = fileFields(21).Replace("""", "")
                        Hillsborough_County_Exemption_Amount_1(count) = fileFields(22).Replace("""", "")
                        Hillsborough_County_Exemption_Code_2(count) = fileFields(23).Replace("""", "")
                        Hillsborough_County_Exemption_Amount_2(count) = fileFields(24).Replace("""", "")
                        Hillsborough_County_Exemption_Code_3(count) = fileFields(25).Replace("""", "")
                        Hillsborough_County_Exemption_Amount_3(count) = fileFields(26).Replace("""", "")
                        Hillsborough_County_Exemption_Code_4(count) = fileFields(27).Replace("""", "")
                        Hillsborough_County_Exemption_Amount_4(count) = fileFields(28).Replace("""", "")
                        Hillsborough_County_Exemption_Code_5(count) = fileFields(29).Replace("""", "")
                        Hillsborough_County_Exemption_Amount_5(count) = fileFields(30).Replace("""", "")
                        Hillsborough_County_AR_Receipt_Date(count) = fileFields(31).Replace("""", "")
                        Hillsborough_County_Certificate_Number(count) = fileFields(32).Replace("""", "")
                        Hillsborough_County_Face_Amount(count) = fileFields(33).Replace("""", "")
                        Hillsborough_County_Sale_Date(count) = fileFields(34).Replace("""", "")
                        Hillsborough_County_Interest_Rate(count) = fileFields(35).Replace("""", "")
                        Hillsborough_County_Legal_Description(count) = fileFields(36).Replace("""", "")
                        count = count + 1

                    Catch ex As Exception

                        Console.WriteLine(ex.ToString())
                    End Try


                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        Console.WriteLine(count)
        Console.WriteLine(Hillsborough_County_Folio_Number(1))
        Console.WriteLine(Hillsborough_County_Sale_Date(1))
        Console.WriteLine(Hillsborough_County_Folio_Number(342182))
        Console.WriteLine(Hillsborough_County_Sale_Date(342182))

        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\HillsBorough_ufc_formatted.txt")
        writeCsv.Flush()
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
        For i As Integer = 342177 To 342182 'Hillborough_record_length - 1
            UFC_Parcel_County = "Hillsborough"
            UFC_Parcel_ID = Hillsborough_County_Folio_Number(i).Trim
            Console.WriteLine(UFC_Parcel_ID)
            UFC_Parcel_Status = ""
            UFC_Parcel_Tax_District = Hillsborough_County_District_Code(i).Trim
            UFC_Parcel_Just_Value = Hillsborough_County_Just_Value(i).Trim
            UFC_Parcel_Taxable_Value = -1
            UFC_Parcel_Street_Address_1 = Hillsborough_County_Location_Street_Number(i).Trim & " " &
            Hillsborough_County_Location_Direction(i).Trim & " " &
            Hillsborough_County_Location_Street_Name(i).Trim & " " &
            Hillsborough_County_Location_Designator(i).Trim & " " &
            Hillsborough_County_Location_Suite(i).Trim & " " &
            Hillsborough_County_Location_City(i).Trim & " " &
            Hillsborough_County_Location_Zip(i).Trim

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
                tempAddress = Hillsborough_County_Location_Street_Number(i).Trim & " " &
                Hillsborough_County_Location_Direction(i).Trim & " " &
                Hillsborough_County_Location_Street_Name(i).Trim & " " &
                Hillsborough_County_Location_Designator(i).Trim & " " &
                Hillsborough_County_Location_Suite(i).Trim & " " &
                Hillsborough_County_Location_City(i).Trim & " " &
                Hillsborough_County_Location_Zip(i).Trim

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
            UFC_Parcel_Owner_Street_Address_1 = ""
            UFC_Parcel_Owner_Street_Address_2 = ""
            UFC_Parcel_Owner_Street_Address_3 = ""
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
            UFC_Parcel_Exemption_Type_1 = Hillsborough_County_Exemption_Code_1(i).Trim
            UFC_Parcel_Exemption_Type_2 = Hillsborough_County_Exemption_Code_2(i).Trim
            UFC_Parcel_Exemption_Type_3 = Hillsborough_County_Exemption_Code_3(i).Trim
            UFC_Parcel_Exemption_Type_4 = Hillsborough_County_Exemption_Code_4(i).Trim
            UFC_Parcel_Exemption_Type_5 = Hillsborough_County_Exemption_Code_5(i).Trim
            UFC_Parcel_Exemption_Type_6 = ""
            If Hillsborough_County_Exemption_Amount_1(i).Trim = "" Then
                UFC_Parcel_Exemption_Amount_1 = -1
            Else
                UFC_Parcel_Exemption_Amount_1 = CInt(Hillsborough_County_Exemption_Amount_1(i).Trim)

            End If

            If Hillsborough_County_Exemption_Amount_2(i).Trim = "" Then
                UFC_Parcel_Exemption_Amount_2 = -1
            Else
                UFC_Parcel_Exemption_Amount_2 = CInt(Hillsborough_County_Exemption_Amount_2(i).Trim)

            End If

            If Hillsborough_County_Exemption_Amount_3(i).Trim = "" Then
                UFC_Parcel_Exemption_Amount_3 = -1
            Else
                UFC_Parcel_Exemption_Amount_3 = CInt(Hillsborough_County_Exemption_Amount_3(i).Trim)

            End If

            If Hillsborough_County_Exemption_Amount_4(i).Trim = "" Then
                UFC_Parcel_Exemption_Amount_4 = -1
            Else
                UFC_Parcel_Exemption_Amount_4 = CInt(Hillsborough_County_Exemption_Amount_4(i).Trim)

            End If

            If Hillsborough_County_Exemption_Amount_5(i).Trim = "" Then
                UFC_Parcel_Exemption_Amount_5 = -1
            Else
                UFC_Parcel_Exemption_Amount_5 = CInt(Hillsborough_County_Exemption_Amount_5(i).Trim)

            End If

            UFC_Parcel_Exemption_Amount_6 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Legal_Description = Hillsborough_County_Legal_Description(i).Trim
            UFC_TC_Number = Hillsborough_County_Certificate_Number(i).Trim
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
            UFC_TC_Tax_Year = ""
            If Hillsborough_County_Face_Amount(i).Trim = "" Then
                UFC_TC_Face_Value = 0
            Else
                UFC_TC_Face_Value = CInt(Hillsborough_County_Face_Amount(i).Trim)
            End If

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = Hillsborough_County_Sale_Date(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(tc_saleDate)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Hillsborough_County_AR_Receipt_Date(i).Trim
            If tc_remDate = "/  /" Or tc_remDate = "00/00/2000" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(tc_remDate.Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If

            'Interest rate
            If Hillsborough_County_Interest_Rate(i).Trim = "" Then
                UFC_TC_Interest_Rate = 0.0
            Else
                UFC_TC_Interest_Rate = Hillsborough_County_Interest_Rate(i).Trim

            End If

            'Redem amount (else if)
            UFC_TC_Redemption_Amount = -1

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
