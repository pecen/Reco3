%% Cognos Raw Data
clearvars

newengine = char('2777909 CBE1_560.txt');
%improvement in %
improvement = 7.84 ;

%Selecting the txt file
[FileNameD,PathNameD,FilterIndexD] = uigetfile('X:\Technical_Product_Planning\20_Organisation\YDM_Modularisation\YDMC\50_Personliga_mappar\Enrique Carrasco Mora\1- CO2 Project\CO2 Roadmap\New components\Engines\*.txt','Select folder with engine xml');
engine_name=fullfile(PathNameD,FileNameD);
% fileID_originalengine = fopen(engine_name);
% A = fscanf(fileID_originalengine,'%c')
% fclose(fileID_originalengine )

fid = fopen(engine_name);
tline = fgetl(fid);
i=0;
while ischar(tline)
    i=i+1;
    Engine_txt{i,1} = tline;
    tline = fgetl(fid);
end
fclose(fid);

%First row with data;
if length(find(strcmp(Engine_txt,'<FuelConsumptionMap>')))== 0
    FC_start = find(strcmp(Engine_txt,'-<FuelConsumptionMap>'));
else
    FC_start = find(strcmp(Engine_txt,'<FuelConsumptionMap>'));
end

FC_end = find(strcmp(Engine_txt,'</FuelConsumptionMap>'));

FC_txt = cell(FC_end-FC_start-1,1);
for i =1:length(FC_txt)
FC_txt{i,1} = Engine_txt{FC_start+i,1};
end

%% DAta processing

Engine_FC_New = cell(length(FC_txt),1);
Engine_FC_New_num = zeros(length(FC_txt),1);
Engine_FC_Old = cell(length(FC_txt),1);
Engine_FC_Old_num = zeros(length(FC_txt),1);
Engine_torque= cell(length(FC_txt),1);
Engine_Speed= cell(length(FC_txt),1);

for i=1:length(Engine_FC_Old)
    if ~isempty(FC_txt{i,1})
        k = strfind(FC_txt{i,1},'fuelConsumption');
        j = strfind(FC_txt{i,1},'" torque');
        Engine_FC_Old{i,1} = FC_txt{i,1}(k+17:j-1);
        Engine_FC_Old_num(i,1) = str2num(Engine_FC_Old{i,1});
%                 Engine_FC_New_num(i,1)= round(Engine_FC_Old_num(i,1)*(1-(improvement/100)));
        Engine_FC_New_num(i,1)= Engine_FC_Old_num(i,1)*(1-(improvement/100));
    end
end

%% Printing results
fileID = fopen(newengine,'wt');

for i=1:FC_start
   if ~isempty(Engine_txt{i,1})
       if strcmp(Engine_txt{i,1}(1),'-')
        fprintf(fileID,'%s\n',Engine_txt{i,1}(2:end));   
       else
        fprintf(fileID,'%s\n',Engine_txt{i,1});
       end
   end
end
% fprintf(fileID,'</FuelConsumptionMap>\n');

for i=1:length(Engine_FC_New_num)
   if ~isempty(FC_txt{i,1})
    % <Entry fuelConsumption="0.00" torque="-193.17" engineSpeed="500.00"/>
    k = strfind(FC_txt{i,1},'torque');
    fprintf(fileID,'   <Entry fuelConsumption="%.2f" %s\n',Engine_FC_New_num(i,1),FC_txt{i,1}(k:end) );
   end
end

for i=FC_end:length(Engine_txt)
   if ~isempty(Engine_txt{i,1})
       if strcmp(Engine_txt{i,1}(1),'-')
        fprintf(fileID,'%s\n',Engine_txt{i,1}(2:end));   
       else
        fprintf(fileID,'%s\n',Engine_txt{i,1});
       end
   end
end
fclose(fileID);

