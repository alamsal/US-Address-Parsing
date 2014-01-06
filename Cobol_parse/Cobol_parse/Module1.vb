Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Text
Imports System.Net
Imports System.Xml

'ZP4 Class to validate Florida county address
Public Class ZP4

    Public Declare Sub ZP4StartSession Lib "ZP4.dll" (ByRef session As Integer)
    Public Declare Function ZP4InputOrder Lib "ZP4.dll" (ByVal session As Integer, ByVal s As String) As Integer
    Public Declare Function ZP4OutputOrder Lib "ZP4.dll" (ByVal session As Integer, ByVal s As String) As Integer
    Public Declare Function ZP4Correct Lib "ZP4.dll" (ByVal session As Integer, ByVal inputs As String, ByVal outputs As StringBuilder) As Integer
    Public Declare Sub ZP4TimeLimit Lib "ZP4.dll" (ByVal session As Integer, ByVal milliseconds As Integer)
End Class

Module Module1
    'Parse address
    Function TestRegExp(myPattern As String, myString As String) As Boolean
        'Create objects.
        Dim objRegExp As Regex
        ' Create a regular expression object.
        objRegExp = New Regex(myPattern)

        'Test whether the String can be compared.
        Dim match As Match = objRegExp.Match(myString)
        If match.Success Then
            Return True
        Else
            Return False
        End If

    End Function
    Public Function parseAddress(ByVal input As String) As Collection
        Dim output As New Collection
        If (Regex.IsMatch(input, "^[0-9 ]+$")) Then
            Console.WriteLine("Only numbers in the Address field")
            output.Add("", "Address1")
            output.Add("", "Address2")
            output.Add("", "City")
            output.Add("", "State")
            output.Add("", "Zip")
            Return output
        Else
            input = input.Replace(",", "")
            input = input.Replace("  ", " ")
            input = input.Replace(" CR ", " County Road ")
            input = input.Replace(" SR ", " State Road ")
            Dim splitString() As String = Split(input)
            Dim streetMarker() As String = New String() {"street", "County Road", "Sate Road", "st", "st.", "avenue", "ave", "ave e", "ave.", "blvd", "blvd.", "highway", "hwy", "hwy.", "box", "road", "plz", "madera", "walk", "bend", "rd", "rd.", "lane", "ln", "ln.", "circle", "circ", "cir", "circ.", "court", "ct", "ter", "ct.", "dr", "trl", "pl", "sr", "cr", "gln", "loop", "way", "xing", "us", "cv", "run", "bluff", "pky", "trce", "palm", "trace", "business", "fairway", "broadway", "island", "manor", "overlook", "oak", "majestic"}
            Dim address1 As String
            Dim address2 As String = ""
            Dim city As String
            Dim state As String
            Dim zip As String
            Dim streetMarkerIndex As Integer
            Dim flag As Boolean



            zip = splitString(splitString.Length - 1).ToString()
            flag = TestRegExp("^\d{5}(?:[-\s]\d{4})?$", zip)
            If flag = True Then
                state = splitString(splitString.Length - 2).ToString()
                streetMarkerIndex = getLastIndexOf(splitString, streetMarker) + 1
                Dim sb As New StringBuilder

                For counter As Integer = streetMarkerIndex To splitString.Length - 3
                    sb.Append(splitString(counter) + " ")
                Next counter
                city = RTrim(sb.ToString())
                Dim addressIndex As Integer = 0

                For counter As Integer = 0 To streetMarkerIndex
                    If IsNumeric(splitString(counter)) _
                    Or splitString(counter).ToString.ToLower = "po" _
                    Or splitString(counter).ToString().ToLower().Replace(".", "") = "po" Then
                        addressIndex = counter
                        Exit For
                    End If
                Next counter

                sb = New StringBuilder
                For counter As Integer = addressIndex To streetMarkerIndex - 1
                    sb.Append(splitString(counter) + " ")
                Next counter

                address1 = RTrim(sb.ToString())

                'sb = New StringBuilder

                'If addressIndex = 0 Then
                '    If splitString(splitString.Length - 2).ToString() <> splitString(streetMarkerIndex + 1) Then
                '        For counter As Integer = streetMarkerIndex To splitString.Length - 2
                '            sb.Append(splitString(counter) + " ")
                '        Next counter
                '    End If
                'Else
                '    For counter As Integer = 0 To addressIndex - 1
                '        sb.Append(splitString(counter) + " ")
                '    Next counter
                'End If
                'address2 = RTrim(sb.ToString())

                output.Add(address1, "Address1")
                'output.Add(address2, "Address2")
                output.Add(city, "City")
                output.Add(state, "State")
                output.Add(zip, "Zip")
                Return output
            Else
                output.Add(input, "Address1")
                output.Add("", "Address2")
                output.Add("", "City")
                output.Add("FL", "State")
                output.Add("", "Zip")
                Return output
            End If
        End If


        
    End Function
    Private Function getLastIndexOf(ByVal sArray As String(), ByVal checkArray As String()) As Integer
        Dim sourceIndex As Integer = 0
        Dim outputIndex As Integer = 0
        For Each item As String In checkArray
            For Each source As String In sArray
                If source.ToLower = item.ToLower Then
                    outputIndex = sourceIndex
                    If item.ToLower = "box" Then
                        outputIndex = outputIndex + 1
                    End If
                End If
                sourceIndex = sourceIndex + 1
            Next
            sourceIndex = 0
        Next
        Return outputIndex
    End Function
    'Validate address using ZP4 software
    Public Function parseZP4(ByVal address As String, ByVal city As String, ByVal state As String) As StringBuilder
        Dim addstring As String
        Dim session As Integer = 0
        ZP4.ZP4StartSession(session)
        If (session = 0) Then
            Console.WriteLine("Session allocation failed!")
        End If
        'ZP4.ZP4TimeLimit(session, 0)
        If (ZP4.ZP4InputOrder(session, "Address" & vbTab & "City" & vbTab & "State") = 0) Then
            Console.WriteLine("Invalid input list!")
        End If

        If (ZP4.ZP4OutputOrder(session, "Address (final)" & vbTab & "City (final)" & vbTab & "State (final)" & vbTab & "ZIP (final)" & vbTab & "ZIP (five-digit)" & vbTab & "ZIP (four-digit add-on)" & vbTab & "Error message") = 0) Then
            Console.WriteLine("Invalid output list!")
        End If

        Dim sb As StringBuilder = New StringBuilder(5000)
        ' mutable string buffer for result
        'Dim address As String = "492 OAK CANOPY CIR AVON PARK"
        'Dim city As String = ""
        'Dim state As String = ""
        addstring = address.Trim & vbTab & city.Trim & vbTab & state.Trim

        If (ZP4.ZP4Correct(session, addstring.Trim, sb) = 0) Then
            Console.WriteLine("Correction call failed!")
        End If
        'Dim s() As String = sb.ToString.Split(vbTab)
        Return sb
    End Function
    'Validate address using Google Geocoding API
    Public Function GetGoogleAddress(ByVal address1 As String, ByVal city As String, ByVal state As String) As Hashtable

        Dim addressxml As String = String.Empty
        Dim zipCode As String = String.Empty
        Dim m_hashTable As New Hashtable()
        m_hashTable.Clear()
        Try
            address1 = address1.Replace(" CR ", " County Road ")
            address1 = address1.Replace(" SR ", " State Road ")
            System.Threading.Thread.Sleep(500)
            'Create an object of web client
            Dim wsClient As New WebClient()

            'Construct the URL concating the address values with it
            Dim zipcodeurl As String = "?address={0},+{1},+{2}&sensor=false"
            'Here in constructing the URL sensor(is required) indicates whether or not the geocoding request comes from a device with a location sensor.

            Dim url As String = "http://maps.googleapis.com/maps/api/geocode/xml" & zipcodeurl
            url = [String].Format(url, address1.Replace(" ", "+"), city.Replace(" ", "+"), state.Replace(" ", "+"))

            'Download the data in XML format as string by making a web request
            addressxml = wsClient.DownloadString(url)

            'Check if status is OK then proceed
            If addressxml.Contains("OK") Then

                'Check if postal_code section is there in the string then proceed
                If addressxml.Contains("postal_code") Then
                    Dim xmlDoc As New XmlDocument()
                    xmlDoc.LoadXml(addressxml)
                    Dim m_nodelist As XmlNodeList
                    Dim node As XmlNode
                    'Get the list of all address_companent nodes as this component only contans the address information
                    'm_nodelist = node.SelectNodes("/GeocodeResponse/result/address_component")
                    node = xmlDoc.SelectSingleNode("/GeocodeResponse/result")
                    m_nodelist = node.SelectNodes("address_component")
                    'From each component check for the type section for getting the particular postal_code section
                    Dim gcount As Integer = 0 'Just to read first entry of google map
                    For Each m_node In m_nodelist
                        Try
                            'Get the zipLongName Element Value
                            Dim LongName = m_node.ChildNodes.Item(0).InnerText
                            'Get the zipShortName Element Value
                            Dim ShortName = m_node.ChildNodes.Item(1).InnerText
                            'Get the zipType Element Value
                            Dim Type = m_node.ChildNodes.Item(2).InnerText

                            'If the type of the component is postal_code then get the postal code as zipLongName
                            If Type = "street_number" Then
                                m_hashTable.Add("street_number", LongName)
                            End If

                            If Type = "route" Then
                                m_hashTable.Add("street", LongName)
                            End If

                            If Type = "locality" Then
                                m_hashTable.Add("city", LongName)
                            End If
                            If Type = "administrative_area_level_1" Then
                                m_hashTable.Add("state", ShortName)
                            End If
                            If Type = "postal_code" Then
                                zipCode = LongName
                                m_hashTable.Add("zip", LongName)
                            End If
                            gcount = gcount + 1
                        Catch ex As Exception
                            Console.WriteLine(ex.ToString())
                        End Try
                    Next

                End If
            Else
                Console.WriteLine("No OK on XML or Over Quota")
            End If


        Catch ex As WebException

            'Console.WriteErrorLog(ex, "GetZipcode", "CompanyDetailViewerForm")
            Console.WriteLine(ex.ToString())


        End Try

        Return m_hashTable


    End Function
    'Validate address using Bing Geocoding API
    Public Function GetBingAddress(ByVal address1 As String, ByVal city As String, ByVal state As String) As Hashtable
        'Reference:http://msdn.microsoft.com/en-us/library/ff701714.aspx

        Dim addressxml As String = String.Empty
        Dim zipCode As String = String.Empty
        Dim m_hashTable As New Hashtable()
        m_hashTable.Clear()
        Try
            address1 = address1.Replace(" CR ", " County Road ")
            address1 = address1.Replace(" SR ", " State Road ")
            System.Threading.Thread.Sleep(500)
            'Create an object of web client
            Dim wsClient As New WebClient()

            'Construct the URL concating the address values with it
            Dim keyurl As String = "&output=xml&key=AsYeEOt9i7KYcGE7sNUTO7HyNcjLjbqBjfFDKmQVxmtMIHKHgT8gUT8cveb_bCVn" ' aalamsal
            'Dim keyurl As String = "&output=xml&key=Av-QgYyMKibzfmSaN1Xj1GbIDZUv9oMsoNfA0mJysMA36ke0Te6BZxikx_d_SsnD" ' jacks

            'Here in constructing the URL sensor(is required) indicates whether or not the geocoding request comes from a device with a location sensor.

            Dim url As String = " http://dev.virtualearth.net/REST/v1/Locations?q=" & address1.Trim & " " & city.Trim & " " & state.Trim & keyurl.Trim
            'Download the data in XML format as string by making a web request
            addressxml = wsClient.DownloadString(url)

            'Check if status is OK then proceed
            If addressxml.Contains("OK") Then
                'Check if postal_code section is there in the string then proceed
                If addressxml.Contains("PostalCode") Then
                    Dim xmlDoc As New XmlDocument()
                    xmlDoc.LoadXml(addressxml)
                    Dim nsmgr As New XmlNamespaceManager(xmlDoc.NameTable)
                    nsmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1")
                    'Get formatted addresses: Option 1
                    'Get all locations in the response and then extract the formatted address for each location
                    Dim locationElements As XmlNodeList = xmlDoc.SelectNodes("//rest:Location", nsmgr)
                    Dim countAddress As Integer = 0
                    For Each location As XmlNode In locationElements
                        If countAddress < 1 Then
                            Try
                                Dim formatedAddress As String = location.SelectSingleNode(".//rest:FormattedAddress", nsmgr).InnerText
                                Dim splitfAdress() As String = formatedAddress.Split(",")

                                m_hashTable.Add("street", location.SelectSingleNode(".//rest:AddressLine", nsmgr).InnerText)
                                m_hashTable.Add("city", splitfAdress(1))
                                m_hashTable.Add("state", location.SelectSingleNode(".//rest:AdminDistrict", nsmgr).InnerText)
                                m_hashTable.Add("zip", location.SelectSingleNode(".//rest:PostalCode", nsmgr).InnerText)
                                countAddress = countAddress + 1
                            Catch ex As Exception
                                Console.WriteLine(ex.ToString)
                            End Try

                        End If

                    Next

                End If
            Else
                Console.WriteLine("No OK on XML or Over Quota")
            End If


        Catch ex As WebException

            'Console.WriteErrorLog(ex, "GetZipcode", "CompanyDetailViewerForm")
            Console.WriteLine(ex.ToString())


        End Try

        Return m_hashTable
    End Function
    'Get Validated address from ZP4,Google Geocoding API & Bing API
    Public Function GetCorrectedAddress(ByVal address As String) As Hashtable
        Dim googleAddress, bingAddress As Hashtable
        Dim returnAddress As New Hashtable
        Dim pAddress As Collection
        Dim zp4address As StringBuilder
        Dim zpaddress As String()
        Dim pcity, pstate, pzip, pstaddress, zip4 As String
        Dim zp4ParseStatus As String = ""
        Dim googleParseStatus As String = ""
        Dim tempAddress As String
        Dim oldParcelId As String = ""
        Dim zpFlag As Boolean = True
        Dim UFC_Parcel_City, UFC_Parcel_State, UFC_Parcel_ZIP_Code, UFC_Parcel_Street_Address, UFC_Parcel_ZIP_Code4 As String
        returnAddress.Clear()

        tempAddress = address
        pAddress = parseAddress(tempAddress.Trim)
        pcity = pAddress.Item("City")
        pstate = pAddress.Item("State")
        pzip = pAddress.Item("Zip")
        pstaddress = pAddress.Item("Address1")

        zp4address = parseZP4(pstaddress, pcity, pstate)
        zpaddress = zp4address.ToString.Split(vbTab)

        UFC_Parcel_City = zpaddress(1)
        UFC_Parcel_State = zpaddress(2)
        UFC_Parcel_ZIP_Code = zpaddress(4)
        UFC_Parcel_Street_Address = zpaddress(0)
        UFC_Parcel_ZIP_Code4 = zpaddress(5)
        zip4 = zpaddress(3)
        zp4ParseStatus = zpaddress(6)

        If ((zpaddress(0) = "0") Or (zpaddress(0) = "NO") Or (zpaddress(0) = "")) Then
            UFC_Parcel_City = "Invalid"
            UFC_Parcel_State = "Invalid"
            UFC_Parcel_ZIP_Code = "Invalid"
            UFC_Parcel_Street_Address = "Invalid"
            UFC_Parcel_ZIP_Code4 = "Invalid"
            zpFlag = False
        End If
        'If ZP4 couldn't find the address
        If ((zip4 = " ") Or (zip4 = "") Or (zpaddress(0) = "0") Or (zpaddress(0) = "NO") Or (zpaddress(0) = " ") Or (zpaddress(0) = "") Or (zpaddress(0) = "no") Or (zpaddress(0) = "No")) Then
            'parse using Bing Geocoding API
            bingAddress = GetBingAddress(tempAddress, "", "FL")
            If (bingAddress.Count < 4) Then
                'parse using Google Geocodign API
                googleAddress = GetGoogleAddress(tempAddress.Trim, "", "FL")
                Try
                    If (googleAddress.Count < 5) Then
                        googleParseStatus = "Invalid Address:Google & Bing "
                    Else
                        UFC_Parcel_City = googleAddress("city")
                        UFC_Parcel_State = googleAddress("state")
                        UFC_Parcel_ZIP_Code = googleAddress("zip")
                        UFC_Parcel_Street_Address = googleAddress("street_number") & " " & googleAddress("street")
                        UFC_Parcel_ZIP_Code4 = ""
                        googleParseStatus = "Parse Address:Google "
                    End If
                Catch ex As Exception
                    Console.WriteLine("Invalid Address:Google ")
                End Try
            Else
                UFC_Parcel_City = bingAddress("city")
                UFC_Parcel_State = bingAddress("state")
                UFC_Parcel_ZIP_Code = bingAddress("zip")
                UFC_Parcel_Street_Address = bingAddress("street")
                UFC_Parcel_ZIP_Code4 = ""
                googleParseStatus = "Parse Address:Bing"
            End If
        End If
        'Parse corrected address using ZP4
        If (zpFlag) Then
            zp4address = parseZP4(UFC_Parcel_Street_Address, UFC_Parcel_City, UFC_Parcel_State)
            zpaddress = zp4address.ToString.Split(vbTab)

            UFC_Parcel_City = zpaddress(1)
            UFC_Parcel_State = zpaddress(2)
            UFC_Parcel_ZIP_Code = zpaddress(4)
            UFC_Parcel_Street_Address = zpaddress(0)
            UFC_Parcel_ZIP_Code4 = zpaddress(5)
            zip4 = zpaddress(3)
            zp4ParseStatus = zpaddress(6)
            If (zp4ParseStatus = "") Then
                zp4ParseStatus = "Parse completed:ZP4"
            End If
        End If

        returnAddress.Add("city", UFC_Parcel_City)
        returnAddress.Add("state", UFC_Parcel_State)
        returnAddress.Add("zip", UFC_Parcel_ZIP_Code)
        returnAddress.Add("zip4", UFC_Parcel_ZIP_Code4)
        returnAddress.Add("street", UFC_Parcel_Street_Address)
        returnAddress.Add("googlebingstatus", googleParseStatus)
        returnAddress.Add("zp4status", zp4ParseStatus)
        Return (returnAddress)
    End Function
    'Parse Counties
    Sub Parse_Cobol_Collier_County()
        'parsing collier county
        ' global variables 
        Dim record_length As Integer = 85490

        Dim textLine As String
        ' count file variables
        Dim parcel_no(record_length) As String
        Dim century(record_length) As String
        Dim year(record_length) As String
        Dim split_no(record_length) As String
        Dim strap_no(record_length) As String
        Dim new_parcel(record_length) As String
        Dim status(record_length) As String
        Dim total_split(record_length) As String
        Dim type(record_length) As String
        Dim mileage(record_length) As String
        Dim installment_flag(record_length) As String
        Dim nav_flag(record_length) As String
        Dim homestead_flag(record_length) As String
        Dim deferred_flag(record_length) As String
        Dim environment_flag(record_length) As String
        Dim land_avial_flag(record_length) As String
        Dim tda_init_flag(record_length) As String
        Dim tda_number(record_length) As String
        Dim tda_redem_interest(record_length) As String
        Dim tda_redem_fees(record_length) As String
        Dim buyer_no(record_length) As String
        Dim prev_buyer_no(record_length) As String
        Dim cert_year(record_length) As String
        Dim cert_no(record_length) As String
        Dim cert_split_no(record_length) As String
        Dim issued_year(record_length) As String
        Dim issued_month(record_length) As String
        Dim issued_day(record_length) As String
        Dim current_cert_amount(record_length) As String
        Dim original_cert_amount(record_length) As String
        Dim percent(record_length) As String
        Dim receipt_no(record_length) As String
        Dim receipt_year(record_length) As String
        Dim receipt_month(record_length) As String
        Dim receipt_day(record_length) As String
        Dim paid_date(record_length) As String
        Dim redem_amount(record_length) As String
        Dim redem_fee(record_length) As String
        Dim transfer_fee(record_length) As String
        Dim transfer_interest(record_length) As String
        Dim accrued_interest(record_length) As String
        Dim Assessed_value(record_length) As String
        Dim unpaid_tax(record_length) As String
        Dim gross_tad(record_length) As String
        Dim ad_valorem_amount(record_length) As String
        Dim penalty(record_length) As String
        Dim commission(record_length) As String
        Dim advertising(record_length) As String
        Dim cert_cost(record_length) As String
        Dim cer_machine_no(record_length) As String
        Dim owner_name(record_length) As String
        Dim address1(record_length) As String
        Dim address2(record_length) As String
        Dim address3(record_length) As String
        Dim address4(record_length) As String
        Dim city(record_length) As String
        Dim state(record_length) As String
        Dim country(record_length) As String
        Dim types(record_length) As String
        Dim zipcode(record_length) As String
        Dim zip_4(record_length) As String
        Dim car_route(record_length) As String
        Dim car_order(record_length) As String
        Dim physical_city_code(record_length) As String
        Dim physical_street(record_length) As String
        Dim physical_street_type(record_length) As String
        Dim physical_ordin(record_length) As String
        Dim physical_street_no(record_length) As String
        Dim physical_street_Xno(record_length) As String
        Dim physical_apt_no(record_length) As String
        Dim legal1(record_length) As String
        Dim legal2(record_length) As String
        Dim legal3(record_length) As String
        Dim legal4(record_length) As String
        Dim legal_lines(record_length) As String
        Dim legal_tranum(record_length) As String
        Dim paid_by1(record_length) As String
        Dim paid_by2(record_length) As String
        Dim paid_by3(record_length) As String
        Dim paid_by4(record_length) As String
        Dim input_by(record_length) As String
        Dim input_date(record_length) As String
        Dim school_taxalbe(record_length) As String
        Dim seniour_taxable(record_length) As String
        Dim payment_method(record_length) As String
        Dim filler(record_length) As String
        'record counter
        Dim count As Integer = 1
        'read the file location

        Dim FILE_NAME As String = "G:\FreeLance\John_Elance\Week2\Collier County - TCs.txt"
        If System.IO.File.Exists(FILE_NAME) = True Then

            Dim objReader As New System.IO.StreamReader(FILE_NAME)

            Do While objReader.Peek() <> -1

                If count <= record_length Then

                    textLine = objReader.ReadLine() & vbNewLine
                    'parsing the lines
                    parcel_no(count) = textLine.Substring(0, 16)

                    century(count) = textLine.Substring(16, 2)
                    year(count) = textLine.Substring(18, 2)

                    split_no(count) = textLine.Substring(20, 2)
                    strap_no(count) = textLine.Substring(22, 18)
                    new_parcel(count) = textLine.Substring(40, 16)
                    status(count) = textLine.Substring(56, 1)
                    total_split(count) = textLine.Substring(57, 2)
                    type(count) = textLine.Substring(59, 1)
                    mileage(count) = textLine.Substring(60, 4)
                    installment_flag(count) = textLine.Substring(64, 2)

                    nav_flag(count) = textLine.Substring(66, 1)
                    homestead_flag(count) = textLine.Substring(67, 1)
                    deferred_flag(count) = textLine.Substring(68, 1)
                    environment_flag(count) = textLine.Substring(69, 1)
                    land_avial_flag(count) = textLine.Substring(70, 1)
                    tda_init_flag(count) = textLine.Substring(71, 1)
                    tda_number(count) = textLine.Substring(72, 6)
                    tda_redem_interest(count) = textLine.Substring(78, 10)
                    tda_redem_fees(count) = textLine.Substring(88, 10)
                    buyer_no(count) = textLine.Substring(98, 4)
                    prev_buyer_no(count) = textLine.Substring(102, 4)
                    cert_year(count) = textLine.Substring(106, 4)
                    cert_no(count) = textLine.Substring(110, 5)
                    cert_split_no(count) = textLine.Substring(115, 2)
                    issued_year(count) = textLine.Substring(117, 4)
                    issued_month(count) = textLine.Substring(121, 2)
                    issued_day(count) = textLine.Substring(123, 2)

                    current_cert_amount(count) = textLine.Substring(125, 10)
                    original_cert_amount(count) = textLine.Substring(135, 10)
                    percent(count) = textLine.Substring(145, 4)
                    receipt_no(count) = textLine.Substring(149, 8)
                    receipt_year(count) = textLine.Substring(157, 2)
                    receipt_month(count) = textLine.Substring(159, 2)
                    receipt_day(count) = textLine.Substring(161, 2)
                    paid_date(count) = textLine.Substring(163, 6)
                    redem_amount(count) = textLine.Substring(169, 10)
                    redem_fee(count) = textLine.Substring(179, 10)
                    transfer_fee(count) = textLine.Substring(189, 10)
                    transfer_interest(count) = textLine.Substring(199, 10)
                    accrued_interest(count) = textLine.Substring(209, 10)
                    Assessed_value(count) = textLine.Substring(219, 10)
                    unpaid_tax(count) = textLine.Substring(229, 10)
                    gross_tad(count) = textLine.Substring(239, 10)
                    ad_valorem_amount(count) = textLine.Substring(249, 10)
                    penalty(count) = textLine.Substring(259, 10)
                    commission(count) = textLine.Substring(269, 10)
                    advertising(count) = textLine.Substring(279, 10)
                    cert_cost(count) = textLine.Substring(289, 10)

                    cer_machine_no(count) = textLine.Substring(299, 2)
                    owner_name(count) = textLine.Substring(301, 30)
                    address1(count) = textLine.Substring(331, 30)
                    address2(count) = textLine.Substring(361, 30)
                    address3(count) = textLine.Substring(391, 30)
                    address4(count) = textLine.Substring(421, 30)
                    city(count) = textLine.Substring(451, 16)
                    state(count) = textLine.Substring(467, 2)
                    country(count) = textLine.Substring(469, 30)
                    types(count) = textLine.Substring(499, 1)
                    zipcode(count) = textLine.Substring(500, 5)
                    zip_4(count) = textLine.Substring(505, 4)
                    car_route(count) = textLine.Substring(509, 4)
                    car_order(count) = textLine.Substring(513, 2)
                    physical_city_code(count) = textLine.Substring(515, 2)
                    physical_street(count) = textLine.Substring(517, 15)
                    physical_street_type(count) = textLine.Substring(532, 4)
                    physical_ordin(count) = textLine.Substring(536, 2)
                    physical_street_no(count) = textLine.Substring(538, 6)
                    physical_street_Xno(count) = textLine.Substring(544, 3)
                    physical_apt_no(count) = textLine.Substring(547, 8)

                    legal1(count) = textLine.Substring(555, 30)
                    legal2(count) = textLine.Substring(585, 30)
                    legal3(count) = textLine.Substring(615, 30)
                    legal4(count) = textLine.Substring(645, 30)
                    legal_lines(count) = textLine.Substring(675, 2)
                    legal_tranum(count) = textLine.Substring(677, 2)
                    paid_by1(count) = textLine.Substring(679, 30)
                    paid_by2(count) = textLine.Substring(709, 30)
                    paid_by3(count) = textLine.Substring(739, 30)
                    paid_by4(count) = textLine.Substring(769, 30)

                    input_by(count) = textLine.Substring(799, 17)
                    input_date(count) = textLine.Substring(816, 6)
                    school_taxalbe(count) = textLine.Substring(822, 10)
                    seniour_taxable(count) = textLine.Substring(832, 10)
                    payment_method(count) = textLine.Substring(842, 2)
                    filler(count) = textLine.Substring(844, 6)

                    count = count + 1

                End If

            Loop
            Console.WriteLine("Test Results from global array")
            Console.WriteLine(parcel_no(1))
            Console.WriteLine(parcel_no(85490))
            Console.WriteLine(payment_method(1))
            Console.WriteLine(payment_method(85490))

            Console.WriteLine("Ready to write outputs in UFC file ...")
            Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\collier_ufc_formatted_csv.txt")

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
            Dim googleAddress, bingAddress As Hashtable
            Dim pAddress As Collection
            Dim zp4address As StringBuilder
            Dim zpaddress As String()
            Dim pcity, pstate, pzip, pstaddress, zip4 As String
            Dim zp4ParseStatus As String = ""
            Dim googleParseStatus As String = ""
            Dim tempAddress As String
            Dim oldParcelId As String = ""

            For i As Integer = 1 To record_length - 1

                UFC_Parcel_County = "Collier"

                UFC_Parcel_ID = parcel_no(i).Trim
                Console.WriteLine(UFC_Parcel_ID)
                UFC_Parcel_Status = status(i).Trim
                Select Case UFC_Parcel_Status
                    Case "Paid in full"
                        UFC_Parcel_Status = "Paid"
                    Case ""
                        UFC_Parcel_Status = ""
                    Case Else
                        UFC_Parcel_Status = UFC_Parcel_Status
                End Select

                UFC_Parcel_Tax_District = ""
                UFC_Parcel_Just_Value = -1
                UFC_Parcel_Taxable_Value = -1
                UFC_Parcel_Street_Address_1 = physical_apt_no(i).Trim
                UFC_Parcel_Street_Address_2 = physical_street_no(i).Trim
                UFC_Parcel_Street_Address_3 = physical_street(i).Trim
                UFC_Parcel_Street_Address_4 = physical_street_type(i).Trim
                UFC_Parcel_Street_Address_5 = physical_street_Xno(i).Trim
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
                    tempAddress = physical_apt_no(i).Trim & " " & physical_street_no(i).Trim & " " & physical_street(i).Trim & " " & physical_street_type(i).Trim & " " & physical_street_Xno(i).Trim
                    pAddress = parseAddress(tempAddress.Trim)
                    pcity = pAddress.Item("City")
                    pstate = pAddress.Item("State")
                    pzip = pAddress.Item("Zip")
                    pstaddress = pAddress.Item("Address1")

                    zp4address = parseZP4(pstaddress, pcity, pstate)
                    zpaddress = zp4address.ToString.Split(vbTab)

                    UFC_Parcel_City = zpaddress(1)
                    UFC_Parcel_State = zpaddress(2)
                    UFC_Parcel_ZIP_Code = zpaddress(4)
                    UFC_Parcel_Street_Address = zpaddress(0)
                    UFC_Parcel_ZIP_Code4 = zpaddress(5)
                    zip4 = zpaddress(3)
                    zp4ParseStatus = zpaddress(6)
                    If ((zpaddress(0) = "0") Or (zpaddress(0) = "NO") Or (zpaddress(0) = "")) Then
                        UFC_Parcel_Street_Address_6 = "Invalid"
                        UFC_Parcel_City = "Invalid"
                        UFC_Parcel_State = "Invalid"
                        UFC_Parcel_ZIP_Code = "Invalid"
                        UFC_Parcel_Street_Address = "Invalid"
                        UFC_Parcel_ZIP_Code4 = "Invalid"
                    End If
                    'If ZP4 couldn't find the address
                    If ((zip4 = " ") Or (zip4 = "") Or (zpaddress(0) = "0") Or (zpaddress(0) = "NO") Or (zpaddress(0) = " ") Or (zpaddress(0) = "") Or (zpaddress(0) = "no") Or (zpaddress(0) = "No")) Then
                        'parse using Bing Geocoding API
                        bingAddress = GetBingAddress(tempAddress, "", "FL")
                        If (bingAddress.Count < 4) Then
                            'parse using Google Geocodign API
                            googleAddress = GetGoogleAddress(tempAddress.Trim, "", "FL")
                            Try
                                If (googleAddress.Count < 5) Then
                                    googleParseStatus = "Invalid Address:Google & Bing "
                                Else
                                    UFC_Parcel_City = googleAddress("city")
                                    UFC_Parcel_State = googleAddress("state")
                                    UFC_Parcel_ZIP_Code = googleAddress("zip")
                                    UFC_Parcel_Street_Address = googleAddress("street_number") & " " & googleAddress("street")
                                    UFC_Parcel_ZIP_Code4 = ""
                                    googleParseStatus = "Parse Address:Google "
                                End If
                            Catch ex As Exception
                                Console.WriteLine("Invalid Address:Google ")
                            End Try
                        Else
                            UFC_Parcel_City = bingAddress("city")
                            UFC_Parcel_State = bingAddress("state")
                            UFC_Parcel_ZIP_Code = bingAddress("zip")
                            UFC_Parcel_Street_Address = bingAddress("street")
                            UFC_Parcel_ZIP_Code4 = ""
                            googleParseStatus = "Parse Address:Bing"
                        End If
                    End If

                End If



                UFC_Parcel_Owner_First_Name = owner_name(i).Trim
                UFC_Parcel_Owner_Last_Name = ""
                UFC_Parcel_Owner_Street_Address_1 = address1(i).Trim
                UFC_Parcel_Owner_Street_Address_2 = address2(i).Trim
                UFC_Parcel_Owner_Street_Address_3 = address3(i).Trim
                UFC_Parcel_Owner_Street_Address_4 = address4(i).Trim
                UFC_Parcel_Owner_Street_Address_5 = ""
                UFC_Parcel_Owner_Street_Address_6 = ""
                UFC_Parcel_Owner_Street_Address = ""
                UFC_Parcel_Owner_City = city(i).Trim
                UFC_Parcel_Owner_State = state(i).Trim
                UFC_Parcel_Owner_ZIP_Code = zipcode(i).Trim
                UFC_Parcel_Owner_ZIP_Code4 = zip_4(i).Trim
                UFC_Parcel_Owner_Country = ""
                UFC_Parcel_Num_of_Exemptions = -1
                UFC_Parcel_Total_Ex_Amount = -1
                UFC_Parcel_Exemption_Type_1 = homestead_flag(i).Trim
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
                UFC_Parcel_Legal_Description = legal1(i).Trim & " " & legal2(i).Trim & " " & legal3(i).Trim & " " & legal4(i).Trim
                UFC_TC_Number = cert_no(i).Trim
                UFC_TC_Status = status(i).Trim
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
                UFC_TC_Issue_Year = cert_year(i).Trim ' need to strip years only

                UFC_TC_Tax_Year = ""
                UFC_TC_Face_Value = cert_cost(i).Trim

                'Date time data processing
                Dim culture As New CultureInfo("en-US")
                'sale date
                Dim tc_saleDate As String
                Dim ds As String = issued_year(i).Trim
                ds = Right(century(i) & ds, 4)

                tc_saleDate = issued_month(i).Trim & "/" & issued_day(i).Trim & "/" & ds
                If tc_saleDate = "/  /" Or tc_saleDate = "00/00/0000" Or tc_saleDate = "00/00/1900" Then
                    UFC_TC_Sale_Date = Date.MinValue
                ElseIf tc_saleDate = "" Or tc_saleDate = "00/00/00" Then
                    UFC_TC_Sale_Date = Date.MinValue
                Else
                    UFC_TC_Sale_Date = DateTime.Parse(tc_saleDate.Trim)
                    UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
                End If
                'Redem date
                Dim tc_remDate As String
                If CInt(receipt_month(i).Trim) > 12 Then ' Collier coutny has mismatched  data with day of month=31, so prevent from date exception due to invalid  day and month are interchanged.
                    tc_remDate = receipt_day(i).Trim & "/" & receipt_month(i).Trim & "/" & Right(century(i) & receipt_year(i).Trim, 4)
                Else
                    tc_remDate = receipt_month(i).Trim & "/" & receipt_day(i).Trim & "/" & Right(century(i) & receipt_year(i).Trim, 4)

                End If

                If tc_remDate = "/  /" Or tc_remDate = "00/00/2000" Or tc_remDate = "00/00/1900" Then
                    UFC_TC_Redemption_Date = Date.MinValue
                ElseIf tc_remDate = "" Or tc_remDate = "00/00/00" Then
                    UFC_TC_Redemption_Date = Date.MinValue
                Else
                    UFC_TC_Redemption_Date = DateTime.Parse(tc_remDate.Trim)
                    UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
                End If

                UFC_TC_Interest_Rate = percent(i).Trim / 100 ' collier county interest rate should be divided by 100 others are fine.

                If redem_amount(i).Trim = "" Then
                    UFC_TC_Redemption_Amount = -1
                Else
                    UFC_TC_Redemption_Amount = redem_amount(i).Trim

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

        Else
            MsgBox("File Does Not Exist")

        End If
    End Sub
    Sub Parse_Orange_County_DelinquentRealEstateTaxData()
        'Orange_county variable length
        Dim Orange_county_record_length As Integer = 167390

        'variables
        Dim Orange_County_Certificate_Year(Orange_county_record_length) As String
        Dim Orange_County_Certificate_Number(Orange_county_record_length) As String
        Dim Orange_County_Certificate_Sequence(Orange_county_record_length) As String
        Dim Orange_County_Parcel_Number(Orange_county_record_length) As String
        Dim Orange_County_Tax_Deed_Year(Orange_county_record_length) As String
        Dim Orange_County_Tax_Deed_Number(Orange_county_record_length) As String
        Dim Orange_County_Tax_Deed_Sequence(Orange_county_record_length) As String
        Dim Orange_County_Tax_Deed_Status(Orange_county_record_length) As String
        Dim Orange_County_Tax_Year(Orange_county_record_length) As String
        Dim Orange_County_Status_Code(Orange_county_record_length) As String
        Dim Orange_County_Mill_Code(Orange_county_record_length) As String
        Dim Orange_County_City_Code(Orange_county_record_length) As String
        Dim Orange_County_Installment_Code(Orange_county_record_length) As String
        Dim Orange_County_Gross_Taxes(Orange_county_record_length) As String
        Dim Orange_County_Certificate_Face_Value(Orange_county_record_length) As String
        Dim Orange_County_Total_Value(Orange_county_record_length) As String
        Dim Orange_County_Exempt_Value(Orange_county_record_length) As String
        Dim Orange_County_Taxable_Value(Orange_county_record_length) As String
        Dim Orange_County_Owner_Name_1(Orange_county_record_length) As String
        Dim Orange_County_Owner_Name_2(Orange_county_record_length) As String
        Dim Orange_County_Owner_Name_3(Orange_county_record_length) As String
        Dim Orange_County_Owner_Name_4(Orange_county_record_length) As String
        Dim Orange_County_Owner_Name_5(Orange_county_record_length) As String
        Dim Orange_County_Owner_Address1(Orange_county_record_length) As String
        Dim Orange_County_Owner_Address2(Orange_county_record_length) As String
        Dim Orange_County_Owner_Address3(Orange_county_record_length) As String
        Dim Orange_County_Owner_Address4(Orange_county_record_length) As String
        Dim Orange_County_Owner_Address5(Orange_county_record_length) As String
        Dim Orange_County_Legal_Description(Orange_county_record_length) As String
        Dim Orange_County_Payoff_Date(Orange_county_record_length) As String
        Dim Orange_County_Payoff_Amount_Due(Orange_county_record_length) As String
        Dim Orange_County_Payoff_Interest(Orange_county_record_length) As String
        Dim Orange_County_Payoff_Amount_Due_Next_Month(Orange_county_record_length) As String
        Dim Orange_County_Payoff_Interest_Next_Month(Orange_county_record_length) As String
        Dim Orange_County_Payoff_Interest_Percentage(Orange_county_record_length) As String
        Dim Orange_County_Payment_Date(Orange_county_record_length) As String
        Dim Orange_County_Payment_Code(Orange_county_record_length) As String
        Dim Orange_County_Validation_Number(Orange_county_record_length) As String
        Dim Orange_County_Bidder_Number(Orange_county_record_length) As String
        Dim Orange_County_Buyer_Name1(Orange_county_record_length) As String
        Dim Orange_County_Buyer_Name2(Orange_county_record_length) As String
        Dim Orange_County_Certificate_Issue_Date(Orange_county_record_length) As String
        Dim Orange_County_Certificate_Purchase_Date(Orange_county_record_length) As String
        Dim Orange_County_Tax_Deed_Application_Date(Orange_county_record_length) As String
        Dim Orange_County_Tax_Deed_Redemption_Date(Orange_county_record_length) As String
        'Reading the orange county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Task3\DelinquentRealEstateTaxData.csv"
        Dim count As Integer = 1

        Dim fileRows(), fileFields() As String

        If File.Exists(fileIn) Then
            Dim fileStream As StreamReader = File.OpenText(fileIn)
            fileRows = fileStream.ReadToEnd().Split(Environment.NewLine)
            'if i=0 csv reading with header, i=1 skip the header
            For i As Integer = 1 To fileRows.Length - 1
                fileFields = fileRows(i).Split(",")
                If count < fileRows.Length - 1 Then
                    Orange_County_Certificate_Year(count) = fileFields(0)
                    Orange_County_Certificate_Number(count) = fileFields(1)
                    Orange_County_Certificate_Sequence(count) = fileFields(2)
                    Orange_County_Parcel_Number(count) = fileFields(3)
                    Orange_County_Tax_Deed_Year(count) = fileFields(4)
                    Orange_County_Tax_Deed_Number(count) = fileFields(5)
                    Orange_County_Tax_Deed_Sequence(count) = fileFields(6)
                    Orange_County_Tax_Deed_Status(count) = fileFields(7)
                    Orange_County_Tax_Year(count) = fileFields(8)
                    Orange_County_Status_Code(count) = fileFields(9)
                    Orange_County_Mill_Code(count) = fileFields(10)
                    Orange_County_City_Code(count) = fileFields(11)
                    Orange_County_Installment_Code(count) = fileFields(12)
                    Orange_County_Gross_Taxes(count) = fileFields(13)
                    Orange_County_Certificate_Face_Value(count) = fileFields(14)
                    Orange_County_Total_Value(count) = fileFields(15)
                    Orange_County_Exempt_Value(count) = fileFields(16)
                    Orange_County_Taxable_Value(count) = fileFields(17)
                    Orange_County_Owner_Name_1(count) = fileFields(18)
                    Orange_County_Owner_Name_2(count) = fileFields(19)
                    Orange_County_Owner_Name_3(count) = fileFields(20)
                    Orange_County_Owner_Name_4(count) = fileFields(21)
                    Orange_County_Owner_Name_5(count) = fileFields(22)
                    Orange_County_Owner_Address1(count) = fileFields(23)
                    Orange_County_Owner_Address2(count) = fileFields(24)
                    Orange_County_Owner_Address3(count) = fileFields(25)
                    Orange_County_Owner_Address4(count) = fileFields(26)
                    Orange_County_Owner_Address5(count) = fileFields(27)
                    Orange_County_Legal_Description(count) = fileFields(28)
                    Orange_County_Payoff_Date(count) = fileFields(29)
                    Orange_County_Payoff_Amount_Due(count) = fileFields(30)
                    Orange_County_Payoff_Interest(count) = fileFields(31)
                    Orange_County_Payoff_Amount_Due_Next_Month(count) = fileFields(32)
                    Orange_County_Payoff_Interest_Next_Month(count) = fileFields(33)
                    Orange_County_Payoff_Interest_Percentage(count) = fileFields(34)
                    Orange_County_Payment_Date(count) = fileFields(35)
                    Orange_County_Payment_Code(count) = fileFields(36)
                    Orange_County_Validation_Number(count) = fileFields(37)
                    Orange_County_Bidder_Number(count) = fileFields(38)
                    Orange_County_Buyer_Name1(count) = fileFields(39)
                    Orange_County_Buyer_Name2(count) = fileFields(40)
                    Orange_County_Certificate_Issue_Date(count) = fileFields(41)
                    Orange_County_Certificate_Purchase_Date(count) = fileFields(42)
                    Orange_County_Tax_Deed_Application_Date(count) = fileFields(43)
                    Orange_County_Tax_Deed_Redemption_Date(count) = fileFields(44)
                    count = count + 1
                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If

        Console.WriteLine(Orange_County_Certificate_Issue_Date(167389))
        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\Orange_UFC_formatted_csv.txt")

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
        Dim UFC_Parcel_Street_Address As String
        Dim UFC_Parcel_City As String
        Dim UFC_Parcel_State As String
        Dim UFC_Parcel_ZIP_Code As String
        Dim UFC_Parcel_ZIP_Code4 As String
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
        Dim googleAddress, bingAddress As Hashtable
        Dim pAddress As Collection
        Dim zp4address As StringBuilder
        Dim zpaddress As String()
        Dim pcity, pstate, pzip, pstaddress, zip4 As String
        Dim zp4ParseStatus As String = ""
        Dim googleParseStatus As String = ""
        Dim tempAddress As String
        Dim oldParcelId As String = ""
        For i As Integer = 1 To Orange_county_record_length - 1

            UFC_Parcel_County = "Orange"

            UFC_Parcel_ID = Orange_County_Parcel_Number(i).Trim
            Console.WriteLine(UFC_Parcel_ID)
            UFC_Parcel_Status = ""
            UFC_Parcel_Tax_District = ""
            UFC_Parcel_Just_Value = -1
            UFC_Parcel_Taxable_Value = Orange_County_Taxable_Value(i).Trim
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
            UFC_Parcel_State = "" 'Need to parse for Alachua & Highlands
            UFC_Parcel_ZIP_Code = ""
            UFC_Parcel_ZIP_Code4 = ""
            UFC_Parcel_Owner_First_Name = Orange_County_Owner_Name_1(i).Trim & " " & Orange_County_Owner_Name_2(i).Trim
            UFC_Parcel_Owner_Last_Name = ""
            UFC_Parcel_Owner_Street_Address_1 = Orange_County_Owner_Address1(i).Trim
            UFC_Parcel_Owner_Street_Address_2 = Orange_County_Owner_Address2(i).Trim
            UFC_Parcel_Owner_Street_Address_3 = Orange_County_Owner_Address3(i).Trim
            UFC_Parcel_Owner_Street_Address_4 = Orange_County_Owner_Address4(i).Trim
            UFC_Parcel_Owner_Street_Address_5 = Orange_County_Owner_Address5(i).Trim
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
            UFC_Parcel_Exemption_Amount_1 = -1
            UFC_Parcel_Exemption_Amount_2 = -1
            UFC_Parcel_Exemption_Amount_3 = -1
            UFC_Parcel_Exemption_Amount_4 = -1
            UFC_Parcel_Exemption_Amount_5 = -1
            UFC_Parcel_Exemption_Amount_6 = -1
            UFC_Parcel_Legal_Description = Orange_County_Legal_Description(i).Trim
            UFC_TC_Number = Orange_County_Certificate_Number(i).Trim
            UFC_TC_Status = Orange_County_Status_Code(i).Trim
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
            UFC_TC_Issue_Year = Orange_County_Certificate_Year(i).Trim ' need to strip years only

            UFC_TC_Tax_Year = Orange_County_Tax_Year(i).Trim
            UFC_TC_Face_Value = Orange_County_Certificate_Face_Value(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = Orange_County_Certificate_Purchase_Date(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(Orange_County_Certificate_Purchase_Date(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Orange_County_Payoff_Date(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(Orange_County_Payoff_Date(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If

            UFC_TC_Interest_Rate = Orange_County_Payoff_Interest_Percentage(i).Trim

            'If Highlands_County_Redemption_Amt(i).Trim = "" Then
            '    UFC_TC_Redemption_Amount = -1
            'Else
            '    UFC_TC_Redemption_Amount = Highlands_County_Redemption_Amt(i).Trim

            'End If
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
    Sub Parse_St_Johns_County()
        'St_Johns_county variable length
        Dim St_Johns_County_record_length As Integer = 30636

        'variables
        Dim St_Johns_County_SYSTEM_DATE(St_Johns_County_record_length) As String
        Dim St_Johns_County_ACCT(St_Johns_County_record_length) As String
        Dim St_Johns_County_FOLIO(St_Johns_County_record_length) As String
        Dim St_Johns_County_GEO_NO(St_Johns_County_record_length) As String
        Dim St_Johns_County_TAX_YEAR(St_Johns_County_record_length) As String
        Dim St_Johns_County_CERT_YEAR(St_Johns_County_record_length) As String
        Dim St_Johns_County_CERT_NUMBER(St_Johns_County_record_length) As String
        Dim St_Johns_County_OWNR_ADDR1(St_Johns_County_record_length) As String
        Dim St_Johns_County_OWNR_ADDR2(St_Johns_County_record_length) As String
        Dim St_Johns_County_OWNR_ADDR3(St_Johns_County_record_length) As String
        Dim St_Johns_County_OWNR_ADDR4(St_Johns_County_record_length) As String
        Dim St_Johns_County_OWNR_ADDR5(St_Johns_County_record_length) As String
        Dim St_Johns_County_OWNR_ADDR6(St_Johns_County_record_length) As String
        Dim St_Johns_County_OWNR_ADDR7(St_Johns_County_record_length) As String
        Dim St_Johns_County_OWNR_ZIP_CODE(St_Johns_County_record_length) As String
        Dim St_Johns_County_STATUSCODE(St_Johns_County_record_length) As String
        Dim St_Johns_County_STATUSDESC(St_Johns_County_record_length) As String
        Dim St_Johns_County_VALUE(St_Johns_County_record_length) As String
        Dim St_Johns_County_FACE_AMT(St_Johns_County_record_length) As String
        Dim St_Johns_County_DUE(St_Johns_County_record_length) As String
        Dim St_Johns_County_AMT_PAID(St_Johns_County_record_length) As String
        Dim St_Johns_County_CERT_TYPE(St_Johns_County_record_length) As String
        Dim St_Johns_County_CERT_DATE_SOLD(St_Johns_County_record_length) As String
        Dim St_Johns_County_BIDR_NO(St_Johns_County_record_length) As String
        Dim St_Johns_County_TAXM_ST_NO(St_Johns_County_record_length) As String
        Dim St_Johns_County_TAXM_ST_NAME(St_Johns_County_record_length) As String
        Dim St_Johns_County_TAXM_ST_CITY(St_Johns_County_record_length) As String
        Dim St_Johns_County_LEGAL01(St_Johns_County_record_length) As String
        Dim St_Johns_County_LEGAL02(St_Johns_County_record_length) As String
        Dim St_Johns_County_LEGAL03(St_Johns_County_record_length) As String
        Dim St_Johns_County_LEGAL04(St_Johns_County_record_length) As String
        Dim St_Johns_County_BIDR_NAME(St_Johns_County_record_length) As String
        Dim St_Johns_County_BIDR_ADDR1(St_Johns_County_record_length) As String
        Dim St_Johns_County_BIDR_ADDR2(St_Johns_County_record_length) As String
        Dim St_Johns_County_BIDR_ADDR3(St_Johns_County_record_length) As String
        Dim St_Johns_County_BIDR_ADDR4(St_Johns_County_record_length) As String
        Dim St_Johns_County_BID_PERCENT(St_Johns_County_record_length) As String
        Dim St_Johns_County_BIDDER_AMT_DUE(St_Johns_County_record_length) As String
        Dim St_Johns_County_DATE_REDEEMED(St_Johns_County_record_length) As String
        Dim St_Johns_County_RECEIPT_NO(St_Johns_County_record_length) As String
        Dim St_Johns_County_TDA_APP_TYPE(St_Johns_County_record_length) As String
        Dim St_Johns_County_TDA_APP_NAME(St_Johns_County_record_length) As String
        Dim St_Johns_County_TDA_APP_DATE(St_Johns_County_record_length) As String
        Dim St_Johns_County_TDA_APP_NO(St_Johns_County_record_length) As String
        Dim St_Johns_County_TDA_APP_AMT(St_Johns_County_record_length) As String
        Dim St_Johns_County_EXEM_CODE(St_Johns_County_record_length) As String

        'Reading the St Johns county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Task5\St Johns County.csv"
        Dim count As Integer = 1

        Dim fileRows(), fileFields() As String

        If File.Exists(fileIn) Then
            Dim fileStream As StreamReader = File.OpenText(fileIn)
            fileRows = fileStream.ReadToEnd().Split(Environment.NewLine)
            'if i=0 csv reading with header, i=1 skip the header
            For i As Integer = 1 To fileRows.Length - 1
                'fileFields = fileRows(i).Split(",")

                Dim pattern As String = "[\t,](?=(?:[^\""]|\""[^\""]*\"")*$)" 'regular expression to parse csv 
                fileFields = Regex.Split(fileRows(i), pattern)


                If count < fileRows.Length - 1 Then
                    St_Johns_County_SYSTEM_DATE(count) = fileFields(0).Replace("""", "")
                    St_Johns_County_ACCT(count) = fileFields(1).Replace("""", "")
                    St_Johns_County_FOLIO(count) = fileFields(2).Replace("""", "")
                    St_Johns_County_GEO_NO(count) = fileFields(3).Replace("""", "")
                    St_Johns_County_TAX_YEAR(count) = fileFields(4).Replace("""", "")
                    St_Johns_County_CERT_YEAR(count) = fileFields(5).Replace("""", "")
                    St_Johns_County_CERT_NUMBER(count) = fileFields(6).Replace("""", "")
                    St_Johns_County_OWNR_ADDR1(count) = fileFields(7).Replace("""", "")
                    St_Johns_County_OWNR_ADDR2(count) = fileFields(8).Replace("""", "")
                    St_Johns_County_OWNR_ADDR3(count) = fileFields(9).Replace("""", "")
                    St_Johns_County_OWNR_ADDR4(count) = fileFields(10).Replace("""", "")
                    St_Johns_County_OWNR_ADDR5(count) = fileFields(11).Replace("""", "")
                    St_Johns_County_OWNR_ADDR6(count) = fileFields(12).Replace("""", "")
                    St_Johns_County_OWNR_ADDR7(count) = fileFields(13).Replace("""", "")
                    St_Johns_County_OWNR_ZIP_CODE(count) = fileFields(14).Replace("""", "")
                    St_Johns_County_STATUSCODE(count) = fileFields(15).Replace("""", "")
                    St_Johns_County_STATUSDESC(count) = fileFields(16).Replace("""", "")
                    St_Johns_County_VALUE(count) = fileFields(17).Replace("""", "")
                    St_Johns_County_FACE_AMT(count) = fileFields(18).Replace("""", "")
                    St_Johns_County_DUE(count) = fileFields(19).Replace("""", "")
                    St_Johns_County_AMT_PAID(count) = fileFields(20).Replace("""", "")
                    St_Johns_County_CERT_TYPE(count) = fileFields(21).Replace("""", "")
                    St_Johns_County_CERT_DATE_SOLD(count) = fileFields(22).Replace("""", "")
                    St_Johns_County_BIDR_NO(count) = fileFields(23).Replace("""", "")
                    St_Johns_County_TAXM_ST_NO(count) = fileFields(24).Replace("""", "")
                    St_Johns_County_TAXM_ST_NAME(count) = fileFields(25).Replace("""", "")
                    St_Johns_County_TAXM_ST_CITY(count) = fileFields(26).Replace("""", "")
                    St_Johns_County_LEGAL01(count) = fileFields(27).Replace("""", "")
                    St_Johns_County_LEGAL02(count) = fileFields(28).Replace("""", "")
                    St_Johns_County_LEGAL03(count) = fileFields(29).Replace("""", "")
                    St_Johns_County_LEGAL04(count) = fileFields(30).Replace("""", "")
                    St_Johns_County_BIDR_NAME(count) = fileFields(31).Replace("""", "")
                    St_Johns_County_BIDR_ADDR1(count) = fileFields(32).Replace("""", "")
                    St_Johns_County_BIDR_ADDR2(count) = fileFields(33).Replace("""", "")
                    St_Johns_County_BIDR_ADDR3(count) = fileFields(34).Replace("""", "")
                    St_Johns_County_BIDR_ADDR4(count) = fileFields(35).Replace("""", "")
                    St_Johns_County_BID_PERCENT(count) = fileFields(36).Replace("""", "")
                    St_Johns_County_BIDDER_AMT_DUE(count) = fileFields(37).Replace("""", "")
                    St_Johns_County_DATE_REDEEMED(count) = fileFields(38).Replace("""", "")
                    St_Johns_County_RECEIPT_NO(count) = fileFields(39).Replace("""", "")
                    St_Johns_County_TDA_APP_TYPE(count) = fileFields(40).Replace("""", "")
                    St_Johns_County_TDA_APP_NAME(count) = fileFields(41).Replace("""", "")
                    St_Johns_County_TDA_APP_DATE(count) = fileFields(42).Replace("""", "")
                    St_Johns_County_TDA_APP_NO(count) = fileFields(43).Replace("""", "")
                    St_Johns_County_TDA_APP_AMT(count) = fileFields(44).Replace("""", "")
                    St_Johns_County_EXEM_CODE(count) = fileFields(45).Replace("""", "")

                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        'Testing the outputs
        Console.WriteLine(St_Johns_County_ACCT(1))
        Console.WriteLine(St_Johns_County_ACCT(30635))
        Console.WriteLine(St_Johns_County_OWNR_ADDR1(1))
        Console.WriteLine(St_Johns_County_OWNR_ADDR2(1))
        Console.WriteLine(St_Johns_County_OWNR_ADDR3(1))
        Console.WriteLine(St_Johns_County_OWNR_ADDR4(1))
        Console.WriteLine(St_Johns_County_OWNR_ADDR5(1))
        Console.WriteLine(St_Johns_County_OWNR_ADDR6(1))
        Console.WriteLine(St_Johns_County_CERT_DATE_SOLD(1))

        Console.WriteLine(St_Johns_County_OWNR_ZIP_CODE(1))
        Console.WriteLine(St_Johns_County_AMT_PAID(1))
        Console.WriteLine(St_Johns_County_RECEIPT_NO(30635))
        'write to csv
        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\stjohns_UFC_formatted_csv3333.txt")

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
        For i As Integer = 1 To St_Johns_County_record_length - 1

            UFC_Parcel_County = "St Johns"

            UFC_Parcel_ID = St_Johns_County_ACCT(i).Trim
            Console.WriteLine(UFC_Parcel_ID)
            UFC_Parcel_Status = ""
            UFC_Parcel_Tax_District = ""
            UFC_Parcel_Just_Value = -1
            UFC_Parcel_Taxable_Value = St_Johns_County_VALUE(i).Trim
            UFC_Parcel_Street_Address_1 = St_Johns_County_TAXM_ST_NO(i).Trim
            UFC_Parcel_Street_Address_2 = St_Johns_County_TAXM_ST_NAME(i).Trim
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
                tempAddress = St_Johns_County_TAXM_ST_NO(i).Trim & " " & St_Johns_County_TAXM_ST_NAME(i).Trim
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
            UFC_Parcel_Owner_Street_Address_1 = St_Johns_County_OWNR_ADDR1(i).Trim & " " & St_Johns_County_OWNR_ADDR2(i).Trim & " " & St_Johns_County_OWNR_ADDR3(i).Trim & " " & St_Johns_County_OWNR_ADDR4(i).Trim & " " & St_Johns_County_OWNR_ADDR5(i).Trim & " " & St_Johns_County_OWNR_ADDR6(i).Trim & " " & St_Johns_County_OWNR_ADDR7(i).Trim
            UFC_Parcel_Owner_Street_Address_2 = ""
            UFC_Parcel_Owner_Street_Address_3 = ""
            UFC_Parcel_Owner_Street_Address_4 = ""
            UFC_Parcel_Owner_Street_Address_5 = ""
            UFC_Parcel_Owner_Street_Address_6 = ""
            UFC_Parcel_Owner_Street_Address = ""
            UFC_Parcel_Owner_City = ""
            UFC_Parcel_Owner_State = ""
            UFC_Parcel_Owner_ZIP_Code = St_Johns_County_OWNR_ZIP_CODE(i).Trim
            UFC_Parcel_Owner_ZIP_Code4 = ""
            UFC_Parcel_Owner_Country = ""
            UFC_Parcel_Num_of_Exemptions = -1
            UFC_Parcel_Total_Ex_Amount = -1
            UFC_Parcel_Exemption_Type_1 = St_Johns_County_EXEM_CODE(i).Trim
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
            UFC_Parcel_Legal_Description = St_Johns_County_LEGAL01(i).Trim & " " & St_Johns_County_LEGAL02(i).Trim & " " & St_Johns_County_LEGAL03(i).Trim & " " & St_Johns_County_LEGAL04(i).Trim
            UFC_TC_Number = St_Johns_County_CERT_NUMBER(i).Trim
            UFC_TC_Status = St_Johns_County_STATUSCODE(i).Trim
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
            UFC_TC_Issue_Year = St_Johns_County_CERT_YEAR(i).Trim ' need to strip years only
            UFC_TC_Tax_Year = St_Johns_County_TAX_YEAR(i).Trim
            UFC_TC_Face_Value = St_Johns_County_FACE_AMT(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = St_Johns_County_CERT_DATE_SOLD(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(St_Johns_County_CERT_DATE_SOLD(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = St_Johns_County_DATE_REDEEMED(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(St_Johns_County_DATE_REDEEMED(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If

            'Interest rate


            UFC_TC_Interest_Rate = St_Johns_County_BID_PERCENT(i).Trim

            If St_Johns_County_DUE(i).Trim = "" Then
                UFC_TC_Redemption_Amount = -1
            Else
                UFC_TC_Redemption_Amount = St_Johns_County_DUE(i).Trim
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
    Sub Parse_Indian_River_County()
        'Indian river county record length
        Dim Indian_River_County_record_length As Integer = 35094
        'variables
        Dim Indian_River_County_Account_Number(Indian_River_County_record_length) As String
        Dim Indian_River_County_Alternate_Key(Indian_River_County_record_length) As String
        Dim Indian_River_County_Tax_Yr(Indian_River_County_record_length) As String
        Dim Indian_River_County_Assessed_Value(Indian_River_County_record_length) As String
        Dim Indian_River_County_Issued_Date(Indian_River_County_record_length) As String
        Dim Indian_River_County_Cert_no(Indian_River_County_record_length) As String
        Dim Indian_River_County_Face_Amount(Indian_River_County_record_length) As String
        Dim Indian_River_County_Interest_Rate(Indian_River_County_record_length) As String
        Dim Indian_River_County_Cert_Buyer(Indian_River_County_record_length) As String
        Dim Indian_River_County_Bidder_no(Indian_River_County_record_length) As String
        Dim Indian_River_County_County_Held(Indian_River_County_record_length) As String
        Dim Indian_River_County_Balance_Status(Indian_River_County_record_length) As String
        Dim Indian_River_County_Account_Status(Indian_River_County_record_length) As String
        Dim Indian_River_County_Cert_Status(Indian_River_County_record_length) As String
        Dim Indian_River_County_Deed_Status(Indian_River_County_record_length) As String
        Dim Indian_River_County_Deed_Application_Amount(Indian_River_County_record_length) As String
        Dim Indian_River_County_Date_Redeemed(Indian_River_County_record_length) As String
        Dim Indian_River_County_Redemption_Amt(Indian_River_County_record_length) As String
        Dim Indian_River_County_Standard_Flags(Indian_River_County_record_length) As String
        Dim Indian_River_County_Custom_Flags(Indian_River_County_record_length) As String

        'parsing the county file into variables
        'Reading the Indian River  county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Task6\Indian River County.csv"
        Dim count As Integer = 1

        Dim fileRows(), fileFields() As String

        If File.Exists(fileIn) Then
            Dim fileStream As StreamReader = File.OpenText(fileIn)
            fileRows = fileStream.ReadToEnd().Split(Environment.NewLine)
            'if i=0 csv reading with header, i=1 skip the header
            For i As Integer = 1 To fileRows.Length - 1
                'fileFields = fileRows(i).Split(",")

                Dim pattern As String = "[\t,](?=(?:[^\""]|\""[^\""]*\"")*$)" 'regular expression to parse csv 
                fileFields = Regex.Split(fileRows(i), pattern)


                If count < fileRows.Length - 1 Then

                    Indian_River_County_Account_Number(count) = fileFields(0).Replace("""", "")
                    Indian_River_County_Alternate_Key(count) = fileFields(1).Replace("""", "")
                    Indian_River_County_Tax_Yr(count) = fileFields(2).Replace("""", "")
                    Indian_River_County_Assessed_Value(count) = fileFields(3).Replace("""", "")
                    Indian_River_County_Issued_Date(count) = fileFields(4).Replace("""", "")
                    Indian_River_County_Cert_no(count) = fileFields(5).Replace("""", "")
                    Indian_River_County_Face_Amount(count) = fileFields(6).Replace("""", "")
                    Indian_River_County_Interest_Rate(count) = fileFields(7).Replace("""", "")
                    Indian_River_County_Cert_Buyer(count) = fileFields(8).Replace("""", "")
                    Indian_River_County_Bidder_no(count) = fileFields(9).Replace("""", "")
                    Indian_River_County_County_Held(count) = fileFields(10).Replace("""", "")
                    Indian_River_County_Balance_Status(count) = fileFields(11).Replace("""", "")
                    Indian_River_County_Account_Status(count) = fileFields(12).Replace("""", "")
                    Indian_River_County_Cert_Status(count) = fileFields(13).Replace("""", "")
                    Indian_River_County_Deed_Status(count) = fileFields(14).Replace("""", "")
                    Indian_River_County_Deed_Application_Amount(count) = fileFields(15).Replace("""", "")
                    Indian_River_County_Date_Redeemed(count) = fileFields(16).Replace("""", "")
                    Indian_River_County_Redemption_Amt(count) = fileFields(17).Replace("""", "")
                    Indian_River_County_Standard_Flags(count) = fileFields(18).Replace("""", "")
                    Indian_River_County_Custom_Flags(count) = fileFields(19).Replace("""", "")

                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        'Testing o/p arrays
        Console.WriteLine(Indian_River_County_Account_Number(1))
        Console.WriteLine(Indian_River_County_Account_Number(35093))
        Console.WriteLine(Indian_River_County_Balance_Status(1))
        Console.WriteLine(Indian_River_County_Date_Redeemed(1))
        Console.WriteLine(Indian_River_County_Redemption_Amt(1))
        Console.WriteLine(Indian_River_County_Redemption_Amt(35093))
        Console.WriteLine("Ready to write outputs in UFC file ...")
        'write into csv
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\Indian_UFC_formatted_csv.txt")

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
        Dim UFC_Parcel_Street_Address As String
        Dim UFC_Parcel_City As String
        Dim UFC_Parcel_State As String
        Dim UFC_Parcel_ZIP_Code As String
        Dim UFC_Parcel_ZIP_Code4 As String
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



        writeCsv.WriteLine("ParcelCounty~ParcelID~ParcelStatus~ParcelTaxDistrict~ParcelJustValue~ParcelTaxableValue~ParcelStreetAddress1~ParcelStreetAddress2~ParcelStreetAddress3~ParcelStreetAddress4~ParcelStreetAddress5~ParcelStreetAddress6~ParcelStreetAddress~ParcelCity~ParcelState~ParcelZIPCode~ParcelZIPCode+4~ParcelOwnerFirstName~ParcelOwnerLastName~ParcelOwnerStreetAddress1~ParcelOwnerStreetAddress2~ParcelOwnerStreetAddress3~ParcelOwnerStreetAddress4~ParcelOwnerStreetAddress5~ParcelOwnerStreetAddress6~ParcelOwnerStreetAddress~ParcelOwnerCity~ParcelOwnerState~ParcelOwnerZIPCode~ParcelOwnerZIPCode+4~ParcelOwnerCountry~ParcelNum.ofExemptions~ParcelTotalEx.Amount~ParcelExemptionType1~ParcelExemptionType2~ParcelExemptionType3~ParcelExemptionType4~ParcelExemptionType5~ParcelExemptionType6~ParcelExemptionAmount1~ParcelExemptionAmount2~ParcelExemptionAmount3~ParcelExemptionAmount4~ParcelExemptionAmount5~ParcelExemptionAmount6~ParcelLegalDescription~TCNumber~TCStatus~TCIssueYear~TCTaxYear~TCFaceValue~TCSaleDate~TCRedemptionDate~TCInterestRate~TCRedemptionAmount~TCDaysUnpaid~ZP4AddressStatus~GoogleAddressStatus")

        For i As Integer = 1 To Indian_River_County_record_length - 1

            UFC_Parcel_County = "Indian River"

            UFC_Parcel_ID = Indian_River_County_Account_Number(i).Trim
            Console.WriteLine(UFC_Parcel_ID)
            UFC_Parcel_Status = Indian_River_County_Account_Status(i).Trim
            Select Case UFC_Parcel_Status
                Case "Paid in full"
                    UFC_Parcel_Status = "Paid"
                Case ""
                    UFC_Parcel_Status = ""
                Case Else
                    UFC_Parcel_Status = UFC_Parcel_Status
            End Select
            UFC_Parcel_Tax_District = ""
            UFC_Parcel_Just_Value = -1
            UFC_Parcel_Taxable_Value = Indian_River_County_Assessed_Value(i).Trim
            UFC_Parcel_Street_Address_1 = ""
            UFC_Parcel_Street_Address_2 = ""
            UFC_Parcel_Street_Address_3 = ""
            UFC_Parcel_Street_Address_4 = ""
            UFC_Parcel_Street_Address_5 = ""
            UFC_Parcel_Street_Address_6 = ""
            UFC_Parcel_Street_Address = ""
            Dim zp4ParseStatus As String = "ZP4:Invalid Address"
            Dim googleParseStatus As String = "Google:Invalid Address"
            UFC_Parcel_City = ""
            UFC_Parcel_State = "" 'Need to parse for Alachua & Highlands
            UFC_Parcel_ZIP_Code = ""
            UFC_Parcel_ZIP_Code4 = ""
            UFC_Parcel_Owner_First_Name = ""
            UFC_Parcel_Owner_Last_Name = ""
            UFC_Parcel_Owner_Street_Address_1 = ""
            UFC_Parcel_Owner_Street_Address_2 = ""
            UFC_Parcel_Owner_Street_Address_3 = ""
            UFC_Parcel_Owner_Street_Address_4 = ""
            UFC_Parcel_Owner_Street_Address_5 = ""
            UFC_Parcel_Owner_Street_Address_6 = ""
            UFC_Parcel_Owner_Street_Address = ""
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
            UFC_Parcel_Exemption_Amount_1 = -1
            UFC_Parcel_Exemption_Amount_2 = -1
            UFC_Parcel_Exemption_Amount_3 = -1
            UFC_Parcel_Exemption_Amount_4 = -1
            UFC_Parcel_Exemption_Amount_5 = -1
            UFC_Parcel_Exemption_Amount_6 = -1
            UFC_Parcel_Legal_Description = -1
            UFC_TC_Number = Indian_River_County_Cert_no(i).Trim
            UFC_TC_Status = Indian_River_County_Cert_Status(i).Trim
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
            UFC_TC_Issue_Year = Indian_River_County_Issued_Date(i).Trim ' need to strip years only
            Dim yearOnly As String()
            yearOnly = Split(UFC_TC_Issue_Year, "/")
            'Console.WriteLine(yearOnly(2))
            UFC_TC_Issue_Year = yearOnly(2)
            UFC_TC_Tax_Year = Indian_River_County_Tax_Yr(i).Trim
            UFC_TC_Face_Value = Indian_River_County_Face_Amount(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = Indian_River_County_Issued_Date(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(Indian_River_County_Issued_Date(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Indian_River_County_Date_Redeemed(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(Indian_River_County_Date_Redeemed(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If


            UFC_TC_Interest_Rate = Indian_River_County_Interest_Rate(i).Trim

            If Indian_River_County_Redemption_Amt(i).Trim = "" Then
                UFC_TC_Redemption_Amount = -1
            Else
                UFC_TC_Redemption_Amount = Indian_River_County_Redemption_Amt(i).Trim

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
    Sub Parse_Highlands_County()
        'Highhlands river county record length
        Dim Highlands_County_record_length As Integer = 112472
        'variables
        Dim Highlands_County_Tax_Yr(Highlands_County_record_length) As String
        Dim Highlands_County_Account_Number(Highlands_County_record_length) As String
        Dim Highlands_County_Cert_no(Highlands_County_record_length) As String
        Dim Highlands_County_Cert_Buyer(Highlands_County_record_length) As String
        Dim Highlands_County_Bidder_no(Highlands_County_record_length) As String
        Dim Highlands_County_Cert_Buyer_Address(Highlands_County_record_length) As String
        Dim Highlands_County_Cert_Status(Highlands_County_record_length) As String
        Dim Highlands_County_Owner_Name(Highlands_County_record_length) As String
        Dim Highlands_County_Owner_Address(Highlands_County_record_length) As String
        Dim Highlands_County_Legal_Desc(Highlands_County_record_length) As String
        Dim Highlands_County_Property_Address(Highlands_County_record_length) As String
        Dim Highlands_County_Exemption(Highlands_County_record_length) As String
        Dim Highlands_County_Account_Status(Highlands_County_record_length) As String
        Dim Highlands_County_Advertised_Number(Highlands_County_record_length) As String
        Dim Highlands_County_Assessed_Value(Highlands_County_record_length) As String
        Dim Highlands_County_Balance_Status(Highlands_County_record_length) As String
        Dim Highlands_County_Cert_Balance_Amt(Highlands_County_record_length) As String
        Dim Highlands_County_County_Held(Highlands_County_record_length) As String
        Dim Highlands_County_Date_Redeemed(Highlands_County_record_length) As String
        Dim Highlands_County_Redemption_Amt(Highlands_County_record_length) As String
        Dim Highlands_County_Receipt_no(Highlands_County_record_length) As String
        Dim Highlands_County_Deed_Status(Highlands_County_record_length) As String
        Dim Highlands_County_Face_Amount(Highlands_County_record_length) As String
        Dim Highlands_County_Interest_Rate(Highlands_County_record_length) As String
        Dim Highlands_County_Issued_Date(Highlands_County_record_length) As String


        'Reading the Highlands county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Task7\Highlands County.csv"
        Dim count As Integer = 1

        Dim fileRows(), fileFields() As String

        If File.Exists(fileIn) Then
            Dim fileStream As StreamReader = File.OpenText(fileIn)
            fileRows = fileStream.ReadToEnd().Split(Environment.NewLine)
            'if i=0 csv reading with header, i=1 skip the header
            For i As Integer = 1 To fileRows.Length - 1
                'fileFields = fileRows(i).Split(",")

                Dim pattern As String = "[\t,](?=(?:[^\""]|\""[^\""]*\"")*$)" 'regular expression to parse csv 
                fileFields = Regex.Split(fileRows(i), pattern)


                If count < fileRows.Length - 1 Then
                    Highlands_County_Tax_Yr(count) = fileFields(0).Replace("""", "")
                    Highlands_County_Account_Number(count) = fileFields(1).Replace("""", "")
                    Highlands_County_Cert_no(count) = fileFields(2).Replace("""", "")
                    Highlands_County_Cert_Buyer(count) = fileFields(3).Replace("""", "")
                    Highlands_County_Bidder_no(count) = fileFields(4).Replace("""", "")
                    Highlands_County_Cert_Buyer_Address(count) = fileFields(5).Replace("""", "")
                    Highlands_County_Cert_Status(count) = fileFields(6).Replace("""", "")
                    Highlands_County_Owner_Name(count) = fileFields(7).Replace("""", "")
                    Highlands_County_Owner_Address(count) = fileFields(8).Replace("""", "")
                    Highlands_County_Legal_Desc(count) = fileFields(9).Replace("""", "")
                    Highlands_County_Property_Address(count) = fileFields(10).Replace("""", "")
                    Highlands_County_Exemption(count) = fileFields(11).Replace("""", "")
                    Highlands_County_Account_Status(count) = fileFields(12).Replace("""", "")
                    Highlands_County_Advertised_Number(count) = fileFields(13).Replace("""", "")
                    Highlands_County_Assessed_Value(count) = fileFields(14).Replace("""", "")
                    Highlands_County_Balance_Status(count) = fileFields(15).Replace("""", "")
                    Highlands_County_Cert_Balance_Amt(count) = fileFields(16).Replace("""", "")
                    Highlands_County_County_Held(count) = fileFields(17).Replace("""", "")
                    Highlands_County_Date_Redeemed(count) = fileFields(18).Replace("""", "")
                    Highlands_County_Redemption_Amt(count) = fileFields(19).Replace("""", "")
                    Highlands_County_Receipt_no(count) = fileFields(20).Replace("""", "")
                    Highlands_County_Deed_Status(count) = fileFields(21).Replace("""", "")
                    Highlands_County_Face_Amount(count) = fileFields(22).Replace("""", "")
                    Highlands_County_Interest_Rate(count) = fileFields(23).Replace("""", "")
                    Highlands_County_Issued_Date(count) = fileFields(24).Replace("""", "")



                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        'test o/p arrays
        Console.WriteLine(Highlands_County_Account_Number(1))
        Console.WriteLine(Highlands_County_Account_Number(112472))
        Console.WriteLine(Highlands_County_Owner_Name(112472))
        Console.WriteLine(Highlands_County_Bidder_no(1))
        Console.WriteLine(Highlands_County_Bidder_no(112472))

        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\Highlands_colon_formatted_csv.txt")

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

        For i As Integer = 1 To Highlands_County_record_length - 1

            UFC_Parcel_County = "Highlands"
            Try
                UFC_Parcel_ID = Highlands_County_Account_Number(i).Trim
                Console.WriteLine(UFC_Parcel_ID)
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
            End Try

            UFC_Parcel_Status = Highlands_County_Account_Status(i).Trim
            Select Case UFC_Parcel_Status
                Case "Paid in full"
                    UFC_Parcel_Status = "Paid"
                Case ""
                    UFC_Parcel_Status = ""
                Case Else
                    UFC_Parcel_Status = UFC_Parcel_Status
            End Select
            UFC_Parcel_Tax_District = ""
            UFC_Parcel_Just_Value = -1
            UFC_Parcel_Taxable_Value = Highlands_County_Assessed_Value(i).Trim
            UFC_Parcel_Street_Address_1 = Highlands_County_Property_Address(i).Trim
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
                tempAddress = Highlands_County_Property_Address(i).Trim
                correctedAddress = GetCorrectedAddress(tempAddress)
                UFC_Parcel_City = correctedAddress("city")
                UFC_Parcel_State = correctedAddress("state")
                UFC_Parcel_ZIP_Code = correctedAddress("zip")
                UFC_Parcel_ZIP_Code4 = correctedAddress("zip4")
                UFC_Parcel_Street_Address = correctedAddress("street")
                googleParseStatus = correctedAddress("googlebingstatus")
                zp4ParseStatus = correctedAddress("zp4status")
            End If

            UFC_Parcel_Owner_First_Name = Highlands_County_Owner_Name(i).Trim
            UFC_Parcel_Owner_Last_Name = ""
            UFC_Parcel_Owner_Street_Address_1 = ""
            UFC_Parcel_Owner_Street_Address_2 = ""
            UFC_Parcel_Owner_Street_Address_3 = ""
            UFC_Parcel_Owner_Street_Address_4 = ""
            UFC_Parcel_Owner_Street_Address_5 = ""
            UFC_Parcel_Owner_Street_Address_6 = ""
            UFC_Parcel_Owner_Street_Address = ""
            UFC_Parcel_Owner_City = ""
            UFC_Parcel_Owner_State = ""
            UFC_Parcel_Owner_ZIP_Code = ""
            UFC_Parcel_Owner_ZIP_Code4 = ""
            UFC_Parcel_Owner_Country = ""
            UFC_Parcel_Num_of_Exemptions = -1
            UFC_Parcel_Total_Ex_Amount = -1
            UFC_Parcel_Exemption_Type_1 = Highlands_County_Exemption(i).Trim
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
            UFC_Parcel_Legal_Description = Highlands_County_Legal_Desc(i).Trim
            UFC_TC_Number = Highlands_County_Cert_no(i).Trim
            UFC_TC_Status = Highlands_County_Cert_Status(i).Trim
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
            UFC_TC_Issue_Year = Highlands_County_Issued_Date(i).Trim ' need to strip years only
            Dim yearOnly As String()
            yearOnly = Split(UFC_TC_Issue_Year, "/")
            UFC_TC_Issue_Year = yearOnly(2)
            UFC_TC_Tax_Year = Highlands_County_Tax_Yr(i).Trim
            UFC_TC_Face_Value = Highlands_County_Face_Amount(i).Trim


            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = Highlands_County_Issued_Date(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(Highlands_County_Issued_Date(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Highlands_County_Date_Redeemed(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(Highlands_County_Date_Redeemed(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If


            UFC_TC_Interest_Rate = Highlands_County_Interest_Rate(i).Trim

            If Highlands_County_Redemption_Amt(i).Trim = "" Then
                UFC_TC_Redemption_Amount = -1
            Else
                UFC_TC_Redemption_Amount = Highlands_County_Redemption_Amt(i).Trim

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
    Sub Parse_Alachua_County()
        'Alachua  county record length
        Dim Alachua_County_record_length As Integer = 34989
        'variables
        Dim Alachua_County_Account_Number(Alachua_County_record_length) As String
        Dim Alachua_County_Owner_Name(Alachua_County_record_length) As String
        Dim Alachua_County_Cert_Buyer(Alachua_County_record_length) As String
        Dim Alachua_County_Tax_Yr(Alachua_County_record_length) As String
        Dim Alachua_County_Cert_no(Alachua_County_record_length) As String
        Dim Alachua_County_Bidder_no(Alachua_County_record_length) As String
        Dim Alachua_County_Account_Status(Alachua_County_record_length) As String
        Dim Alachua_County_Advertised_Number(Alachua_County_record_length) As String
        Dim Alachua_County_Assessed_Value(Alachua_County_record_length) As String
        Dim Alachua_County_Balance_Status(Alachua_County_record_length) As String
        Dim Alachua_County_Cert_Balance_Amt(Alachua_County_record_length) As String
        Dim Alachua_County_Cert_Status(Alachua_County_record_length) As String
        Dim Alachua_County_County_Held(Alachua_County_record_length) As String
        Dim Alachua_County_Date_Redeemed(Alachua_County_record_length) As String
        Dim Alachua_County_Deed_Status(Alachua_County_record_length) As String
        Dim Alachua_County_Delinquent_Costs(Alachua_County_record_length) As String
        Dim Alachua_County_Face_Amount(Alachua_County_record_length) As String
        Dim Alachua_County_Interest_Rate(Alachua_County_record_length) As String
        Dim Alachua_County_Issued_Date(Alachua_County_record_length) As String
        Dim Alachua_County_Receipt_no(Alachua_County_record_length) As String
        Dim Alachua_County_Redemption_Amt(Alachua_County_record_length) As String

        'Reading the Alachua county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Task8\Alachua County.csv"
        Dim count As Integer = 1

        Dim fileRows(), fileFields() As String

        If File.Exists(fileIn) Then
            Dim fileStream As StreamReader = File.OpenText(fileIn)
            fileRows = fileStream.ReadToEnd().Split(Environment.NewLine)
            'if i=0 csv reading with header, i=1 skip the header
            For i As Integer = 1 To fileRows.Length - 1
                'fileFields = fileRows(i).Split(",")

                Dim pattern As String = "[\t,](?=(?:[^\""]|\""[^\""]*\"")*$)" 'regular expression to parse csv 
                fileFields = Regex.Split(fileRows(i), pattern)


                If count < fileRows.Length - 1 Then
                    Alachua_County_Account_Number(count) = fileFields(0).Replace("""", "")
                    Alachua_County_Owner_Name(count) = fileFields(1).Replace("""", "")
                    Alachua_County_Cert_Buyer(count) = fileFields(2).Replace("""", "")
                    Alachua_County_Tax_Yr(count) = fileFields(3).Replace("""", "")
                    Alachua_County_Cert_no(count) = fileFields(4).Replace("""", "")
                    Alachua_County_Bidder_no(count) = fileFields(5).Replace("""", "")
                    Alachua_County_Account_Status(count) = fileFields(6).Replace("""", "")
                    Alachua_County_Advertised_Number(count) = fileFields(7).Replace("""", "")
                    Alachua_County_Assessed_Value(count) = fileFields(8).Replace("""", "")
                    Alachua_County_Balance_Status(count) = fileFields(9).Replace("""", "")
                    Alachua_County_Cert_Balance_Amt(count) = fileFields(10).Replace("""", "")
                    Alachua_County_Cert_Status(count) = fileFields(11).Replace("""", "")
                    Alachua_County_County_Held(count) = fileFields(12).Replace("""", "")
                    Alachua_County_Date_Redeemed(count) = fileFields(13).Replace("""", "")
                    Alachua_County_Deed_Status(count) = fileFields(14).Replace("""", "")
                    Alachua_County_Delinquent_Costs(count) = fileFields(15).Replace("""", "")
                    Alachua_County_Face_Amount(count) = fileFields(16).Replace("""", "")
                    Alachua_County_Interest_Rate(count) = fileFields(17).Replace("""", "")
                    Alachua_County_Issued_Date(count) = fileFields(18).Replace("""", "")
                    Alachua_County_Receipt_no(count) = fileFields(19).Replace("""", "")
                    Alachua_County_Redemption_Amt(count) = fileFields(20).Replace("""", "")


                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        Console.WriteLine(Alachua_County_Account_Number(1))
        Console.WriteLine(Alachua_County_Account_Number(2))
        Console.WriteLine(Alachua_County_Account_Number(34988))
        Console.WriteLine(Alachua_County_Owner_Name(1))
        Console.WriteLine(Alachua_County_Redemption_Amt(1))
        Console.WriteLine(Alachua_County_Redemption_Amt(34988))
        Console.WriteLine("Ready to write outputs in UFC file ...")

        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\Alachua_colon_formatted_csv.txt")

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
        Dim UFC_Parcel_Street_Address As String
        Dim UFC_Parcel_City As String
        Dim UFC_Parcel_State As String
        Dim UFC_Parcel_ZIP_Code As String
        Dim UFC_Parcel_ZIP_Code4 As String
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



        writeCsv.WriteLine("ParcelCounty~ParcelID~ParcelStatus~ParcelTaxDistrict~ParcelJustValue~ParcelTaxableValue~ParcelStreetAddress1~ParcelStreetAddress2~ParcelStreetAddress3~ParcelStreetAddress4~ParcelStreetAddress5~ParcelStreetAddress6~ParcelStreetAddress~ParcelCity~ParcelState~ParcelZIPCode~ParcelZIPCode+4~ParcelOwnerFirstName~ParcelOwnerLastName~ParcelOwnerStreetAddress1~ParcelOwnerStreetAddress2~ParcelOwnerStreetAddress3~ParcelOwnerStreetAddress4~ParcelOwnerStreetAddress5~ParcelOwnerStreetAddress6~ParcelOwnerStreetAddress~ParcelOwnerCity~ParcelOwnerState~ParcelOwnerZIPCode~ParcelOwnerZIPCode+4~ParcelOwnerCountry~ParcelNum.ofExemptions~ParcelTotalEx.Amount~ParcelExemptionType1~ParcelExemptionType2~ParcelExemptionType3~ParcelExemptionType4~ParcelExemptionType5~ParcelExemptionType6~ParcelExemptionAmount1~ParcelExemptionAmount2~ParcelExemptionAmount3~ParcelExemptionAmount4~ParcelExemptionAmount5~ParcelExemptionAmount6~ParcelLegalDescription~TCNumber~TCStatus~TCIssueYear~TCTaxYear~TCFaceValue~TCSaleDate~TCRedemptionDate~TCInterestRate~TCRedemptionAmount~TCDaysUnpaid~ZP4AddressStatus~GoogleAddressStatus")


        For i As Integer = 1 To Alachua_County_record_length - 1

            UFC_Parcel_County = "Alachua"

            UFC_Parcel_ID = Alachua_County_Account_Number(i).Trim

            UFC_Parcel_Status = Alachua_County_Account_Status(i).Trim
            Select Case UFC_Parcel_Status
                Case "Paid in full"
                    UFC_Parcel_Status = "Paid"
                Case ""
                    UFC_Parcel_Status = ""
                Case Else
                    UFC_Parcel_Status = UFC_Parcel_Status
            End Select
            UFC_Parcel_Tax_District = ""
            UFC_Parcel_Just_Value = -1
            UFC_Parcel_Taxable_Value = Alachua_County_Assessed_Value(i).Trim
            UFC_Parcel_Street_Address_1 = ""
            UFC_Parcel_Street_Address_2 = ""
            UFC_Parcel_Street_Address_3 = ""
            UFC_Parcel_Street_Address_4 = ""
            UFC_Parcel_Street_Address_5 = ""
            UFC_Parcel_Street_Address_6 = ""
            UFC_Parcel_Street_Address = ""
            Dim zp4ParseStatus As String = "ZP4:Invalid Address"
            Dim googleParseStatus As String = "Google:Invalid Address"
            UFC_Parcel_City = ""
            UFC_Parcel_State = ""
            UFC_Parcel_ZIP_Code = ""
            UFC_Parcel_ZIP_Code4 = ""
            UFC_Parcel_Owner_First_Name = Alachua_County_Owner_Name(i).Trim
            UFC_Parcel_Owner_Last_Name = ""
            UFC_Parcel_Owner_Street_Address_1 = ""
            UFC_Parcel_Owner_Street_Address_2 = ""
            UFC_Parcel_Owner_Street_Address_3 = ""
            UFC_Parcel_Owner_Street_Address_4 = ""
            UFC_Parcel_Owner_Street_Address_5 = ""
            UFC_Parcel_Owner_Street_Address_6 = ""
            UFC_Parcel_Owner_Street_Address = ""
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
            UFC_Parcel_Exemption_Amount_1 = -1
            UFC_Parcel_Exemption_Amount_2 = -1
            UFC_Parcel_Exemption_Amount_3 = -1
            UFC_Parcel_Exemption_Amount_4 = -1
            UFC_Parcel_Exemption_Amount_5 = -1
            UFC_Parcel_Exemption_Amount_6 = -1
            UFC_Parcel_Legal_Description = ""
            UFC_TC_Number = Alachua_County_Cert_no(i).Trim
            UFC_TC_Status = Alachua_County_Cert_Status(i).Trim
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
            UFC_TC_Issue_Year = Alachua_County_Issued_Date(i).Trim ' need to strip years only
            Dim yearOnly As String()
            yearOnly = Split(UFC_TC_Issue_Year, "/")
            'Console.WriteLine(yearOnly(2))
            UFC_TC_Issue_Year = yearOnly(2)

            UFC_TC_Face_Value = Alachua_County_Face_Amount(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = Alachua_County_Issued_Date(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(Alachua_County_Issued_Date(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Alachua_County_Date_Redeemed(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(Alachua_County_Date_Redeemed(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If

            'Interest rate
            UFC_TC_Tax_Year = Alachua_County_Tax_Yr(i).Trim

            'Redem amount
            If Alachua_County_Redemption_Amt(i).Trim = "" Then
                UFC_TC_Redemption_Amount = -1
            Else
                UFC_TC_Redemption_Amount = Alachua_County_Redemption_Amt(i).Trim
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
    Sub Parse_Manatee_County()
        'Manaatee  county record length
        Dim Manatee_County_record_length As Integer = 62159
        'variables
        Dim Manatee_County_PARCEL(Manatee_County_record_length) As String
        Dim Manatee_County_STATUS(Manatee_County_record_length) As String
        Dim Manatee_County_OWNER_1ST(Manatee_County_record_length) As String
        Dim Manatee_County_BIDDER(Manatee_County_record_length) As String
        Dim Manatee_County_TAXDIST(Manatee_County_record_length) As String
        Dim Manatee_County_INST_FLAG(Manatee_County_record_length) As String
        Dim Manatee_County_JUST_VAL(Manatee_County_record_length) As String
        Dim Manatee_County_EX_TYPE1(Manatee_County_record_length) As String
        Dim Manatee_County_EX_TYPE2(Manatee_County_record_length) As String
        Dim Manatee_County_EX_TYPE3(Manatee_County_record_length) As String
        Dim Manatee_County_EX_TYPE4(Manatee_County_record_length) As String
        Dim Manatee_County_EX_TYPE5(Manatee_County_record_length) As String
        Dim Manatee_County_EX_TYPE6(Manatee_County_record_length) As String
        Dim Manatee_County_TAXABLE(Manatee_County_record_length) As String
        Dim Manatee_County_TAXBILL(Manatee_County_record_length) As String
        Dim Manatee_County_TAX_YR(Manatee_County_record_length) As String
        Dim Manatee_County_CERT_YR(Manatee_County_record_length) As String
        Dim Manatee_County_CERTNR(Manatee_County_record_length) As String
        Dim Manatee_County_SALE_(Manatee_County_record_length) As String
        Dim Manatee_County_INT_RATE(Manatee_County_record_length) As String
        Dim Manatee_County_FACE_VAL(Manatee_County_record_length) As String
        Dim Manatee_County_RED_AMT(Manatee_County_record_length) As String
        Dim Manatee_County_RED(Manatee_County_record_length) As String
        Dim Manatee_County_RECEIPT(Manatee_County_record_length) As String
        Dim Manatee_County_LEGAL1(Manatee_County_record_length) As String
        Dim Manatee_County_LEGAL2(Manatee_County_record_length) As String
        Dim Manatee_County_PAD(Manatee_County_record_length) As String

        'Reading the Manatee county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Task9\Manatee County1.csv"
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

                    Manatee_County_PARCEL(count) = fileFields(0).Replace("""", "")
                    Manatee_County_STATUS(count) = fileFields(1).Replace("""", "")
                    Manatee_County_OWNER_1ST(count) = fileFields(2).Replace("""", "")
                    Manatee_County_BIDDER(count) = fileFields(3).Replace("""", "")
                    Manatee_County_TAXDIST(count) = fileFields(4).Replace("""", "")
                    Manatee_County_INST_FLAG(count) = fileFields(5).Replace("""", "")
                    Manatee_County_JUST_VAL(count) = fileFields(6).Replace("""", "")
                    Manatee_County_EX_TYPE1(count) = fileFields(7).Replace("""", "")
                    Manatee_County_EX_TYPE2(count) = fileFields(8).Replace("""", "")
                    Manatee_County_EX_TYPE3(count) = fileFields(9).Replace("""", "")
                    Manatee_County_EX_TYPE4(count) = fileFields(10).Replace("""", "")
                    Manatee_County_EX_TYPE5(count) = fileFields(11).Replace("""", "")
                    Manatee_County_EX_TYPE6(count) = fileFields(12).Replace("""", "")
                    Manatee_County_TAXABLE(count) = fileFields(13).Replace("""", "")
                    Manatee_County_TAXBILL(count) = fileFields(14).Replace("""", "")
                    Manatee_County_TAX_YR(count) = fileFields(15).Replace("""", "")
                    Manatee_County_CERT_YR(count) = fileFields(16).Replace("""", "")
                    Manatee_County_CERTNR(count) = fileFields(17).Replace("""", "")
                    Manatee_County_SALE_(count) = fileFields(18).Replace("""", "")
                    Manatee_County_INT_RATE(count) = fileFields(19).Replace("""", "")
                    Manatee_County_FACE_VAL(count) = fileFields(20).Replace("""", "")
                    Manatee_County_RED_AMT(count) = fileFields(21).Replace("""", "")
                    Manatee_County_RED(count) = fileFields(22).Replace("""", "")
                    Manatee_County_RECEIPT(count) = fileFields(23).Replace("""", "")
                    Manatee_County_LEGAL1(count) = fileFields(24).Replace("""", "")
                    Manatee_County_LEGAL2(count) = fileFields(25).Replace("""", "")
                    Manatee_County_PAD(count) = fileFields(26).Replace("""", "")

                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If
        'testing o/p array variables
        Console.WriteLine(Manatee_County_PARCEL(1))
        Console.WriteLine(Manatee_County_PARCEL(62158))
        Console.WriteLine(Manatee_County_LEGAL2(62158))
        Console.WriteLine(Manatee_County_RED_AMT(1))
        Console.WriteLine(Manatee_County_RED_AMT(62158))
        Console.WriteLine(Manatee_County_PAD(62158))

        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\Manatee_address_formatted_csv.txt")

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

        For i As Integer = 1 To Manatee_County_record_length - 1

            UFC_Parcel_County = "Manatee"

            UFC_Parcel_ID = Manatee_County_PARCEL(i).Trim
            Console.WriteLine(UFC_Parcel_ID)
            UFC_Parcel_Status = ""
            UFC_Parcel_Tax_District = Manatee_County_TAXDIST(i).Trim
            UFC_Parcel_Just_Value = Manatee_County_JUST_VAL(i).Trim
            UFC_Parcel_Taxable_Value = Manatee_County_TAXABLE(i).Trim
            UFC_Parcel_Street_Address_1 = Manatee_County_PAD(i).Trim
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
                tempAddress = Manatee_County_PAD(i).Trim
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
            UFC_Parcel_Exemption_Type_1 = Manatee_County_EX_TYPE1(i).Trim
            UFC_Parcel_Exemption_Type_2 = Manatee_County_EX_TYPE2(i).Trim
            UFC_Parcel_Exemption_Type_3 = Manatee_County_EX_TYPE3(i).Trim
            UFC_Parcel_Exemption_Type_4 = Manatee_County_EX_TYPE4(i).Trim
            UFC_Parcel_Exemption_Type_5 = Manatee_County_EX_TYPE5(i).Trim
            UFC_Parcel_Exemption_Type_6 = Manatee_County_EX_TYPE6(i).Trim
            UFC_Parcel_Exemption_Amount_1 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_2 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_3 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_4 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_5 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_6 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Legal_Description = Manatee_County_LEGAL1(i).Trim & " " & Manatee_County_LEGAL2(i).Trim
            UFC_TC_Number = Manatee_County_CERTNR(i).Trim
            UFC_TC_Status = Manatee_County_STATUS(i).Trim

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

            UFC_TC_Issue_Year = Manatee_County_CERT_YR(i).Trim ' need to strip years only
            UFC_TC_Tax_Year = Manatee_County_TAX_YR(i).Trim
            UFC_TC_Face_Value = Manatee_County_FACE_VAL(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = Manatee_County_SALE_(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(Manatee_County_SALE_(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Manatee_County_RED(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(Manatee_County_RED(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If

            'Interest rate
            UFC_TC_Interest_Rate = Manatee_County_INT_RATE(i).Trim

            'Redem amount
            If Manatee_County_RED_AMT(i).Trim = "" Then
                UFC_TC_Redemption_Amount = -1
            Else
                UFC_TC_Redemption_Amount = Manatee_County_RED_AMT(i).Trim
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
    Sub Parse_Clay_County()
        Dim Clay_County_record_length As Integer = 31837
        'variables
        Dim Clay_County_ACCT(Clay_County_record_length) As String
        Dim Clay_County_VALUE(Clay_County_record_length) As String
        Dim Clay_County_TAXM_ST_NO(Clay_County_record_length) As String
        Dim Clay_County_TAXM_ST_NAME(Clay_County_record_length) As String
        Dim Clay_County_OWNR_ADDR1(Clay_County_record_length) As String
        Dim Clay_County_OWNR_ADDR2(Clay_County_record_length) As String
        Dim Clay_County_OWNR_ADDR3(Clay_County_record_length) As String
        Dim Clay_County_OWNR_ADDR4(Clay_County_record_length) As String
        Dim Clay_County_OWNR_ADDR5(Clay_County_record_length) As String
        Dim Clay_County_OWNR_ADDR6(Clay_County_record_length) As String
        Dim Clay_County_OWNR_ADDR7(Clay_County_record_length) As String
        Dim Clay_County_OWNR_ZIP_CODE(Clay_County_record_length) As String
        Dim Clay_County_LEGAL01(Clay_County_record_length) As String
        Dim Clay_County_LEGAL02(Clay_County_record_length) As String
        Dim Clay_County_LEGAL03(Clay_County_record_length) As String
        Dim Clay_County_LEGAL04(Clay_County_record_length) As String
        Dim Clay_County_CERT_NUMBER(Clay_County_record_length) As String
        Dim Clay_County_CERT_YEAR(Clay_County_record_length) As String
        Dim Clay_County_TAX_YEAR(Clay_County_record_length) As String
        Dim Clay_County_FACE_AMT(Clay_County_record_length) As String
        Dim Clay_County_CERT_DATE_SOLD(Clay_County_record_length) As String
        Dim Clay_County_DATE_REDEEMED(Clay_County_record_length) As String
        Dim Clay_County_BID_PERCENT(Clay_County_record_length) As String

        'Reading the Manatee county csv file
        Dim fileIn As String = "G:\FreeLance\John_Elance\Week3\Task10\Clay County.csv"
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

                    Clay_County_ACCT(count) = fileFields(0).Replace("""", "")
                    Clay_County_VALUE(count) = fileFields(1).Replace("""", "")
                    Clay_County_TAXM_ST_NO(count) = fileFields(2).Replace("""", "")
                    Clay_County_TAXM_ST_NAME(count) = fileFields(3).Replace("""", "")
                    Clay_County_OWNR_ADDR1(count) = fileFields(4).Replace("""", "")
                    Clay_County_OWNR_ADDR2(count) = fileFields(5).Replace("""", "")
                    Clay_County_OWNR_ADDR3(count) = fileFields(6).Replace("""", "")
                    Clay_County_OWNR_ADDR4(count) = fileFields(7).Replace("""", "")
                    Clay_County_OWNR_ADDR5(count) = fileFields(8).Replace("""", "")
                    Clay_County_OWNR_ADDR6(count) = fileFields(9).Replace("""", "")
                    Clay_County_OWNR_ADDR7(count) = fileFields(10).Replace("""", "")
                    Clay_County_OWNR_ZIP_CODE(count) = fileFields(11).Replace("""", "")
                    Clay_County_LEGAL01(count) = fileFields(12).Replace("""", "")
                    Clay_County_LEGAL02(count) = fileFields(13).Replace("""", "")
                    Clay_County_LEGAL03(count) = fileFields(14).Replace("""", "")
                    Clay_County_LEGAL04(count) = fileFields(15).Replace("""", "")
                    Clay_County_CERT_NUMBER(count) = fileFields(16).Replace("""", "")
                    Clay_County_CERT_YEAR(count) = fileFields(17).Replace("""", "")
                    Clay_County_TAX_YEAR(count) = fileFields(18).Replace("""", "")
                    Clay_County_FACE_AMT(count) = fileFields(19).Replace("""", "")
                    Clay_County_CERT_DATE_SOLD(count) = fileFields(20).Replace("""", "")
                    Clay_County_DATE_REDEEMED(count) = fileFields(21).Replace("""", "")
                    Clay_County_BID_PERCENT(count) = fileFields(22).Replace("""", "")
                    count = count + 1

                End If
            Next
        Else
            Console.WriteLine("File not found")
        End If

        Console.WriteLine(Clay_County_ACCT(1))
        Console.WriteLine(Clay_County_BID_PERCENT(1))
        Console.WriteLine(Clay_County_CERT_NUMBER(1))
        Console.WriteLine(Clay_County_FACE_AMT(1))
        Console.WriteLine(Clay_County_ACCT(31836))
        Console.WriteLine(Clay_County_BID_PERCENT(31836))
        Console.WriteLine(Clay_County_CERT_NUMBER(31836))
        Console.WriteLine(Clay_County_FACE_AMT(31836))

        Console.WriteLine("Ready to write outputs in UFC file ...")
        Dim writeCsv As New StreamWriter("G:\FreeLance\John_Elance\Week4\Clay_ufc_formatted2916.txt")

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

        For i As Integer = 1 To Clay_County_record_length - 1
            UFC_Parcel_County = "Clay"
            UFC_Parcel_ID = Clay_County_ACCT(i).Trim
            Console.WriteLine(UFC_Parcel_ID)
            UFC_Parcel_Status = ""
            UFC_Parcel_Tax_District = ""
            UFC_Parcel_Just_Value = -1
            UFC_Parcel_Taxable_Value = Clay_County_VALUE(i).Trim
            UFC_Parcel_Street_Address_1 = Clay_County_TAXM_ST_NO(i).Trim
            UFC_Parcel_Street_Address_2 = Clay_County_TAXM_ST_NAME(i).Trim
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
                tempAddress = Clay_County_TAXM_ST_NO(i).Trim & " " & Clay_County_TAXM_ST_NAME(i).Trim
                correctedAddress = GetCorrectedAddress(tempAddress)
                UFC_Parcel_City = correctedAddress("city")
                UFC_Parcel_State = correctedAddress("state")
                UFC_Parcel_ZIP_Code = correctedAddress("zip")
                UFC_Parcel_ZIP_Code4 = correctedAddress("zip4")
                UFC_Parcel_Street_Address = correctedAddress("street")
                googleParseStatus = correctedAddress("googlebingstatus")
                zp4ParseStatus = correctedAddress("zp4status")
            End If


            If Clay_County_OWNR_ADDR4(i).Trim = "" Then
                UFC_Parcel_Owner_First_Name = Clay_County_OWNR_ADDR1(i).Trim
            Else
                UFC_Parcel_Owner_First_Name = Clay_County_OWNR_ADDR1(i).Trim & " " & Clay_County_OWNR_ADDR2(i).Trim

            End If
            If Clay_County_OWNR_ADDR7(i).Trim = "" Then
                If Clay_County_OWNR_ADDR6(i).Trim = "" Then
                    If Clay_County_OWNR_ADDR5(i).Trim = "" Then
                        If Clay_County_OWNR_ADDR4(i).Trim = "" Then
                            UFC_Parcel_Owner_First_Name = Clay_County_OWNR_ADDR1(i).Trim
                            UFC_Parcel_Owner_Street_Address_1 = Clay_County_OWNR_ADDR2(i).Trim
                            UFC_Parcel_Owner_Street_Address_2 = Clay_County_OWNR_ADDR3(i).Trim
                            UFC_Parcel_Owner_Street_Address_3 = ""
                            UFC_Parcel_Owner_Street_Address_4 = ""
                            UFC_Parcel_Owner_Street_Address_5 = ""
                            UFC_Parcel_Owner_Street_Address_6 = ""
                        Else
                            UFC_Parcel_Owner_First_Name = Clay_County_OWNR_ADDR1(i).Trim & " " & Clay_County_OWNR_ADDR2(i).Trim
                            UFC_Parcel_Owner_Street_Address_1 = Clay_County_OWNR_ADDR3(i).Trim
                            UFC_Parcel_Owner_Street_Address_2 = Clay_County_OWNR_ADDR4(i).Trim
                            UFC_Parcel_Owner_Street_Address_3 = ""
                            UFC_Parcel_Owner_Street_Address_4 = ""
                            UFC_Parcel_Owner_Street_Address_5 = ""
                            UFC_Parcel_Owner_Street_Address_6 = ""
                        End If
                    Else
                        UFC_Parcel_Owner_First_Name = Clay_County_OWNR_ADDR1(i).Trim & " " & Clay_County_OWNR_ADDR2(i).Trim & " " & Clay_County_OWNR_ADDR3(i).Trim
                        UFC_Parcel_Owner_Street_Address_1 = Clay_County_OWNR_ADDR4(i).Trim
                        UFC_Parcel_Owner_Street_Address_2 = Clay_County_OWNR_ADDR5(i).Trim
                        UFC_Parcel_Owner_Street_Address_3 = ""
                        UFC_Parcel_Owner_Street_Address_4 = ""
                        UFC_Parcel_Owner_Street_Address_5 = ""
                        UFC_Parcel_Owner_Street_Address_6 = ""
                    End If
                Else
                    UFC_Parcel_Owner_First_Name = Clay_County_OWNR_ADDR1(i).Trim & " " & Clay_County_OWNR_ADDR2(i).Trim & " " & Clay_County_OWNR_ADDR3(i).Trim & " " & Clay_County_OWNR_ADDR4(i).Trim
                    UFC_Parcel_Owner_Street_Address_1 = Clay_County_OWNR_ADDR4(i).Trim
                    UFC_Parcel_Owner_Street_Address_2 = Clay_County_OWNR_ADDR6(i).Trim
                    UFC_Parcel_Owner_Street_Address_3 = ""
                    UFC_Parcel_Owner_Street_Address_4 = ""
                    UFC_Parcel_Owner_Street_Address_5 = ""
                    UFC_Parcel_Owner_Street_Address_6 = ""
                End If
            Else
                UFC_Parcel_Owner_First_Name = Clay_County_OWNR_ADDR1(i).Trim & " " & Clay_County_OWNR_ADDR2(i).Trim
                UFC_Parcel_Owner_Street_Address_1 = Clay_County_OWNR_ADDR2(i).Trim
                UFC_Parcel_Owner_Street_Address_2 = Clay_County_OWNR_ADDR7(i).Trim
                UFC_Parcel_Owner_Street_Address_3 = ""
                UFC_Parcel_Owner_Street_Address_4 = ""
                UFC_Parcel_Owner_Street_Address_5 = ""
                UFC_Parcel_Owner_Street_Address_6 = ""
            End If


            UFC_Parcel_Owner_Last_Name = ""
            UFC_Parcel_Owner_street_Address = ""
            UFC_Parcel_Owner_City = ""
            UFC_Parcel_Owner_State = ""
            UFC_Parcel_Owner_ZIP_Code = Clay_County_OWNR_ZIP_CODE(i).Trim
            UFC_Parcel_Owner_ZIP_Code4 = ""
            UFC_Parcel_Owner_Country = ""
            UFC_Parcel_Num_of_Exemptions = -1
            UFC_Parcel_Total_Ex_Amount = -1
            UFC_Parcel_Exemption_Type_1 = -1
            UFC_Parcel_Exemption_Type_2 = -1
            UFC_Parcel_Exemption_Type_3 = -1
            UFC_Parcel_Exemption_Type_4 = -1
            UFC_Parcel_Exemption_Type_5 = -1
            UFC_Parcel_Exemption_Type_6 = -1
            UFC_Parcel_Exemption_Amount_1 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_2 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_3 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_4 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_5 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Exemption_Amount_6 = -1 'CInt(Manatee_County_EX_TYPE1(i).Trim)
            UFC_Parcel_Legal_Description = Clay_County_LEGAL01(i).Trim & " " & Clay_County_LEGAL02(i).Trim & " " & Clay_County_LEGAL03(i).Trim & " " & Clay_County_LEGAL04(i).Trim
            UFC_TC_Number = Clay_County_CERT_NUMBER(i).Trim
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

            UFC_TC_Issue_Year = Clay_County_CERT_YEAR(i).Trim ' need to strip years only
            UFC_TC_Tax_Year = Clay_County_TAX_YEAR(i).Trim
            UFC_TC_Face_Value = Clay_County_FACE_AMT(i).Trim

            'Date time data processing
            Dim culture As New CultureInfo("en-US")
            'sale date
            Dim tc_saleDate As String
            tc_saleDate = Clay_County_CERT_DATE_SOLD(i).Trim
            If tc_saleDate = "/  /" Then
                UFC_TC_Sale_Date = Date.MinValue
            ElseIf tc_saleDate = "" Then
                UFC_TC_Sale_Date = Date.MinValue
            Else
                UFC_TC_Sale_Date = DateTime.Parse(Clay_County_CERT_DATE_SOLD(i).Trim)
                UFC_TC_Sale_Date = Format(UFC_TC_Sale_Date, "M/d/yyyy")
            End If

            'Redem date
            Dim tc_remDate As String
            tc_remDate = Clay_County_DATE_REDEEMED(i).Trim

            If tc_remDate = "/  /" Then
                UFC_TC_Redemption_Date = Date.MinValue
            ElseIf tc_remDate = "" Then
                UFC_TC_Redemption_Date = Date.MinValue
            Else
                UFC_TC_Redemption_Date = DateTime.Parse(Clay_County_DATE_REDEEMED(i).Trim)
                UFC_TC_Redemption_Date = Format(UFC_TC_Redemption_Date, "M/d/yyyy")
            End If

            'Interest rate
            UFC_TC_Interest_Rate = Clay_County_BID_PERCENT(i).Trim

            'Redem amount
            UFC_TC_Redemption_Amount = -1

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
    Sub Main()

        'Parse_Cobol_Collier_County()
        'Parse_Orange_County_DelinquentRealEstateTaxData()
        'Parse_St_Johns_County()
        'Parse_Indian_River_County()
        'Parse_Highlands_County()
        'Parse_Alachua_County()
        'Parse_Manatee_County()
        'Parse_Clay_County()
        'Parse_Columbia_County() 
        'PascoCounty.ParsePascoCounty()
        Columbia.ParseColumbiaCounty()
        'Hillsborough.ParseHillsboroughCounty()
        'SantaRosa.ParseSantaRosaCounty()

        ' '''''''''''''''''''''''''''''''''''''''''''''''''

        'Dim session As Integer = 0
        'ZP4.ZP4StartSession(session)
        'If (session = 0) Then
        '    Console.WriteLine("Session allocation failed!")
        'End If
        ''ZP4.ZP4TimeLimit(session, 0)
        'If (ZP4.ZP4InputOrder(session, "Address" & vbTab & "City" & vbTab & "State") = 0) Then
        '    Console.WriteLine("Invalid input list!")
        'End If

        'If (ZP4.ZP4OutputOrder(session, "Address (final)" & vbTab & "City (final)" & vbTab & "State (final)" & vbTab & "ZIP (final)" & vbTab & "ZIP (five-digit)" & vbTab & "ZIP (four-digit add-on)" & vbTab & "Error message") = 0) Then
        '    Console.WriteLine("Invalid output list!")
        'End If

        'Dim sb As StringBuilder = New StringBuilder(5000)
        '' mutable string buffer for result
        'Dim address As String = "1018 GRAND LAKE BLVD"
        'Dim city As String = "AVON PARK"
        'Dim state As String = ""

        'Dim mystring As String
        'mystring = address.Trim & vbTab & city.Trim & vbTab & state.Trim


        'If (ZP4.ZP4Correct(session, mystring.Trim, sb) = 0) Then
        '    Console.WriteLine("Correction call failed!")
        'End If


        'Dim s() As String = sb.ToString.Split(vbTab)
        ' break out the tab-delimited result fields

        ' ''''''''''''''''''''''''''''''''''''''''''''''''''
        'Dim google_address As Hashtable = GetGoogleAddress("2611 W PHEASANT CT", " ", "FL")
        'Dim bing_address As Hashtable = GetBingAddress("5010 MIRAHAM DR", "", "FL")
        'Dim corrected_address As Hashtable = GetCorrectedAddress("2613  HACIENDA DR LORIDA, FL 33857")

        Console.ReadKey()
    End Sub



End Module
