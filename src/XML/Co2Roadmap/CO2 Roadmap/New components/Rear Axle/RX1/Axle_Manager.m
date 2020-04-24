%% Cognos Raw Data
clearvars

newaxle = char('CAM 5.25-R5.25.xml');
%improvement in %
reduction = 5.250;

%Selecting the txt file
[FileNameD,PathNameD,FilterIndexD] = uigetfile('X:\Technical_Product_Planning\20_Organisation\YDM_Modularisation\YDMC\50_Personliga_mappar\Enrique Carrasco Mora\1- CO2 Project\CO2 Roadmap\New components\Rear Axle\RX1\*.xlsx','Select folder with engine xml');
Axle_name=fullfile(PathNameD,FileNameD);
[Axlenum Axletxt Axleraw]=xlsread(Axle_name);


%% Printing results
fileID = fopen(newaxle,'wt');

fprintf(fileID,'<Axlegear>\n');
fprintf(fileID,' <Data id="axle-HY1344">\n');
fprintf(fileID,' <Manufacturer>MAN</Manufacturer>\n');
fprintf(fileID,' <Model>HY1344</Model>\n');
fprintf(fileID,' <CertificationNumber>e4*2017/2400*2017/2400*L*0032*00</CertificationNumber>\n');
fprintf(fileID,' <Date>2018-02-28T13:15:25.4251948Z</Date>\n');
fprintf(fileID,' <AppVersion>1.2.0.0</AppVersion>\n');
fprintf(fileID,' <LineType>Single reduction axle</LineType>\n');
fprintf(fileID,' <Ratio>%.3f</Ratio>\n',reduction);
fprintf(fileID,' <CertificationMethod>Measured</CertificationMethod>\n');
fprintf(fileID,' <TorqueLossMap>\n');

for i=2:size(Axlenum,2)
for j=2:size(Axlenum,1)
fprintf(fileID,' 	<Entry inputSpeed="%.2f" inputTorque="%.2f" torqueLoss="%.2f"/>\n',Axlenum(1,i),Axlenum(j,1),Axlenum(j,i));  
end 
end

fprintf(fileID,'    </TorqueLossMap>\n');
fprintf(fileID,'   </Data>\n');
fprintf(fileID,'  <Signature>\n');
fprintf(fileID,'  <di:Reference URI="">\n');
fprintf(fileID,'  <di:Transforms>\n');
fprintf(fileID,'  <di:Transform Algorithm="http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithoutComments" />\n');
fprintf(fileID,'  <di:Transform Algorithm="urn: vecto: xml: 2017:canonicalization" />\n');
fprintf(fileID,'  </di:Transforms>\n');
fprintf(fileID,'  <di:DigestMethod Algorithm="http://www.w3.org/2001/04/xmlenc#sha256" />\n');
fprintf(fileID,'  <di:DigestValue></di:DigestValue>\n');
fprintf(fileID,' </di:Reference>\n');
fprintf(fileID,'</Signature>\n');
fprintf(fileID,'</Axlegear>\n');

fclose(fileID);