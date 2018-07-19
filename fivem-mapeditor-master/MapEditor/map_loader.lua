-- This file is part of the FiveMapEditor, it reads xml map files in 5me (MEME) and Guadmaz formats, spawns the entities and gives their handles back to FiveMapEditor.
Citizen.Trace("[mapeditor] Started map_loader.lua")
local function processXml(el) -- source: blattersturm/cfx-object-loader
	local v = {}
	local text

	for _,kid in ipairs(el.kids) do
		if kid.type == 'text' then
			text = kid.value
		elseif kid.type == 'element' then
			if not v[kid.name] then
				v[kid.name] = {}
			end

			table.insert(v[kid.name], processXml(kid))
		end
	end

	v._ = el.attr

	if #el.attr == 0 and #el.el == 0 then
		v = text
	end

	return v
end


local function func_192(sba)
	if sba == 1 then
		return 0.3
	elseif sba == 2 then
		return 0.4
	elseif sba == 3 then
		return 0.5
	elseif sba == 4 then
		return 0.5
	elseif sba == 5 then
		return 0.5
	else
		return 0.4
	end
end

local function func_193(sba)
	if sba == 1 then
		return 15
	elseif sba == 2 then
		return 25
	elseif sba == 3 then
		return 35
	elseif sba == 4 then
		return 45
	elseif sba == 5 then
		return 100
	else
		return 25
	end
end

local function func_190(sba)
	if sba == 1 then
		return 44
	elseif sba == 2 then
		return 30
	elseif sba == 3 then
		return 16
	else
		return 30
	end
end

local function func_191(hash)
	if (hash == 346059280 or hash == 620582592 or hash == 85342060 or hash == 483832101 or hash == 930976262 or hash == 1677872320 or hash == 708828172 or hash == 950795200 or hash == -1260656854 or hash == -1875404158 or hash == -864804458 or hash == -1302470386 or hash == 1518201148 or hash == 384852939 or hash == 117169896 or hash == -1479958115) then
		return 1
	else
		return 0
	end
end

local function joaat(s)
	return GetHashKey(s)
end

local function setSBA(obj, sba)
	local hash = GetEntityModel(obj)
	if (hash == joaat("stt_prop_track_speedup") or hash == joaat("stt_prop_track_speedup_t1") or hash == joaat("stt_prop_track_speedup_t2") or hash == joaat("stt_prop_stunt_tube_speed") or hash == joaat("stt_prop_stunt_tube_speedb")) then
		Citizen.InvokeNative(0x7BAC110ED504814D, obj, func_193(sba))
		Citizen.InvokeNative(0x4E91E2848E9525BB, obj, func_192(sba))
	elseif (hash == joaat("stt_prop_track_slowdown") or hash == joaat("stt_prop_track_slowdown_t1") or hash == joaat("stt_prop_track_slowdown_t2") or func_191(hash))then
		Citizen.InvokeNative(0x7BAC110ED504814D, obj, func_190(sba))
	end
end

local function loadMapForRace(map, callback)
	local entityHandles = {}
	name = map.Name[1]
	creator = map.Creator[1]
	if map.Description[1] ~= nil then
		description = map.Description[1]
	end
	props = map.Props
	if props ~= nil and #props > 0 then
		props = props[1].Prop
		for _,prop in ipairs(props) do
			local hash = tonumber(prop.Hash[1])
			local pos = vector3(tonumber(prop.Pos[1].X[1]),tonumber(prop.Pos[1].Y[1]),tonumber(prop.Pos[1].Z[1]))
			local rot = vector3(tonumber(prop.Rot[1].X[1]),tonumber(prop.Rot[1].Y[1]),tonumber(prop.Rot[1].Z[1]))
			local dynamic = false
			local colorid = 0
			if prop.Color then
				colorid = tonumber(prop.Color[1])
			end
			while not HasModelLoaded(hash) do
				RequestModel(hash)
				Wait(0)
			end
			local obj = CreateObjectNoOffset(hash, pos, true, networkObjects, dynamic)
			SetEntityRotation(obj, rot, 2, true)
			SetObjectTextureVariant(obj, colorid)
			if prop.SBA then
				local sba = tonumber(prop.SBA[1])
				setSBA(obj, sba)
			end
			if hash == GetHashKey("stt_prop_hoop_constraction_01a") then
				local dict = "scr_stunts"
				Citizen.InvokeNative(0x4B10CEA9187AFFE6, dict)
				if Citizen.InvokeNative(0xC7225BF8901834B2, dict) then
					Citizen.InvokeNative(0xFF62C471DC947844, dict)
					Citizen.InvokeNative(0x2FBC377D2B29B60F, "scr_stunts_fire_ring", obj, vector3(0, 0, 25), vector3(-12.5, 0, 0), 1.0, 0,0,0)
				end
			elseif hash == GetHashKey("stt_prop_hoop_small_01") then
				local dict = "core"
				Citizen.InvokeNative(0x4B10CEA9187AFFE6, dict)
				if Citizen.InvokeNative(0xC7225BF8901834B2, dict) then
					Citizen.InvokeNative(0xFF62C471DC947844, dict)
					Citizen.InvokeNative(0x2FBC377D2B29B60F, "ent_amb_fire_ring", obj, vector3(0, 0, 4.5), vector3(0, 0, 90), 3.5, 0,0,0)
				end								
			end
			table.insert(entityHandles, obj)
		end
	end
	callback({name, creator, description, table.unpack(entityHandles)});
end


function loadMap(data, networkObjects, callback)
Citizen.CreateThread(function()
		Citizen.Trace("Are created entities being networked: "..tostring(networkObjects))
		local name, creator, description = "","",""
		local entityHandles = {}
		
		local xml = SLAXML:dom(data)
		if xml and xml.root then
			if xml.root.name == "Race" then
				Citizen.Trace("Parsing as a FiveRace.\n")
				xml = processXml(xml.root)
				local pos = vector3(tonumber(xml.Grid[1].Pos[1].X[1]),tonumber(xml.Grid[1].Pos[1].Y[1]),tonumber(xml.Grid[1].Pos[1].Z[1]))
				map = xml.Map[1]
				loadMapForRace(map, callback)
				cps = xml.Checkpoints[1].Checkpoint
				local previous_cp = nil
				for _, cp in ipairs(cps) do
					local pos = vector3(tonumber(cp.Pos[1].X[1]),tonumber(cp.Pos[1].Y[1]),tonumber(cp.Pos[1].Z[1]))
					if previous_cp == nil then
						previous_cp = pos
					else
						CreateCheckpoint(0, previous_cp.x, previous_cp.y, previous_cp.z, pos.x, pos.y, pos.z, 10.0, 180, 180, 50, 150, 0)
						previous_cp = pos
					end
					
				end
				Citizen.Wait(2000)
				Citizen.Trace("Teleporting to starting grid")
				SetEntityCoords(GetPlayerPed(-1), pos, 1, 0, 0, 0)
			elseif xml.root.name == "Map" then
				if xml.root.attr["version"] then -- FiveMapEditor
					Citizen.Trace("Parsing as a FiveMapEditor map.\n")
					xml = processXml(xml.root)
					name = xml.Name[1]
					creator = xml.Creator[1]
					if xml.Description[1] ~= nil then
						description = xml.Description[1]
					end
					props = xml.Props
					if props ~= nil and #props > 0 then
						props = props[1].Prop
						for _,prop in ipairs(props) do
							local hash = tonumber(prop.Hash[1])
							local pos = vector3(tonumber(prop.Pos[1].X[1]),tonumber(prop.Pos[1].Y[1]),tonumber(prop.Pos[1].Z[1]))
							local rot = vector3(tonumber(prop.Rot[1].X[1]),tonumber(prop.Rot[1].Y[1]),tonumber(prop.Rot[1].Z[1]))
							local dynamic = false
							local colorid = 0
							if prop.Color then
								colorid = tonumber(prop.Color[1])
							end
							while not HasModelLoaded(hash) do
								RequestModel(hash)
								Wait(0)
							end
							local obj = CreateObjectNoOffset(hash, pos, true, networkObjects, dynamic)
							SetEntityRotation(obj, rot, 2, true)
							SetObjectTextureVariant(obj, colorid)
							if prop.SBA then
								local sba = tonumber(prop.SBA[1])
								setSBA(obj, sba)
							end
							if hash == GetHashKey("stt_prop_hoop_constraction_01a") then
								local dict = "scr_stunts"
								Citizen.InvokeNative(0x4B10CEA9187AFFE6, dict)
								if Citizen.InvokeNative(0xC7225BF8901834B2, dict) then
									Citizen.InvokeNative(0xFF62C471DC947844, dict)
									Citizen.InvokeNative(0x2FBC377D2B29B60F, "scr_stunts_fire_ring", obj, vector3(0, 0, 25), vector3(-12.5, 0, 0), 1.0, 0,0,0)
								end
							elseif hash == GetHashKey("stt_prop_hoop_small_01") then
								local dict = "core"
								Citizen.InvokeNative(0x4B10CEA9187AFFE6, dict)
								if Citizen.InvokeNative(0xC7225BF8901834B2, dict) then
									Citizen.InvokeNative(0xFF62C471DC947844, dict)
									Citizen.InvokeNative(0x2FBC377D2B29B60F, "ent_amb_fire_ring", obj, vector3(0, 0, 4.5), vector3(0, 0, 90), 3.5, 0,0,0)
								end								
							end
							table.insert(entityHandles, obj)
						end
					end
					peds = xml.Peds
					if peds ~= nil and #peds > 0 then
					print(peds[1])
						peds = peds[1].Ped
						for _,ped in ipairs(peds) do
							local hash = tonumber(ped.Hash[1])
							local pos = vector3(tonumber(ped.Pos[1].X[1]),tonumber(ped.Pos[1].Y[1]),tonumber(ped.Pos[1].Z[1]))
							local rot = vector3(tonumber(ped.Rot[1].X[1]),tonumber(ped.Rot[1].Y[1]),tonumber(ped.Rot[1].Z[1]))
							local pedtype = tonumber(ped.Type[1])
							while not HasModelLoaded(hash) do
								RequestModel(hash)
								Wait(0)
							end
							local pedhandle = CreatePed(pedtype, hash, pos, rot.z, networkObjects, false)
							--SetEntityRotation(pedhandle, rot, 2, true)
							table.insert(entityHandles, pedhandle)
						end
					end
					vehs = xml.Vehicles
					if vehs ~= nil and #vehs > 0 then
						vehs = vehs[1].Vehicle
						for _,veh in ipairs(vehs) do
							local hash = tonumber(veh.Hash[1])
							local pos = vector3(tonumber(veh.Pos[1].X[1]),tonumber(veh.Pos[1].Y[1]),tonumber(veh.Pos[1].Z[1]))
							local rot = vector3(tonumber(veh.Rot[1].X[1]),tonumber(veh.Rot[1].Y[1]),tonumber(veh.Rot[1].Z[1]))
							while not HasModelLoaded(hash) do
								RequestModel(hash)
								Wait(0)
							end
							local vehhandle = CreateVehicle(hash, pos, 0, networkObjects, false)
							SetEntityRotation(vehhandle, rot, 2, true)
							table.insert(entityHandles, vehhandle)
						end
					end
					callback({name, creator, description, table.unpack(entityHandles)});
					
				else -- Guadmaz's Map Editor
					Citizen.Trace("Parsing as a GuadMaz Map Editor map.\n")
					xml = processXml(xml.root)
					if xml.Metadata then
						name = xml.Metadata[1].Name[1]
						creator = xml.Metadata[1].Creator[1]
						description = xml.Metadata[1].Description[1]
					end
					props = xml.Objects[1].MapObject
					propcount = 0
					for _,prop in ipairs(props) do
						if prop.Type[1] == "Prop" then
							propcount = propcount +1
							local hash = tonumber(prop.Hash[1])
							local pos = vector3(tonumber(prop.Position[1].X[1]),tonumber(prop.Position[1].Y[1]),tonumber(prop.Position[1].Z[1]))
							local rot = vector3(tonumber(prop.Rotation[1].X[1]),tonumber(prop.Rotation[1].Y[1]),tonumber(prop.Rotation[1].Z[1]))
							local dynamic = prop.Dynamic[1] == "true"
							local obj = CreateObjectNoOffset(hash, pos, true, networkObjects, dynamic)
							SetEntityRotation(obj, rot, 2, true)
							table.insert(propHandles, obj)
						end
					end
					Citizen.Trace("This map contained " .. propcount .. " objects of which " .. #propHandles.." were spawned.")
					callback({name, creator, description, table.unpack(propHandles)})
				end
			end
		end
	end)
end