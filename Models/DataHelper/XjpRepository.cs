using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace Models.DataHelper
{
    /// <summary>
    /// 数据访问层，封装访问EF操作
    /// </summary>
    public class XjpRepository
    {
        private StreetContext _context;
        public XjpRepository()
        {
            _context = new StreetContext();
        }

        public XjpRepository(StreetContext context)
        {
            _context = context;
        }

        #region street
        public void AddStreet(StreetUnit street)
        {
            _context.Streets.Add(street);
        }

        public void DeleteStreet(StreetUnit street)
        {
            _context.Streets.Remove(street);
        }

        public StreetUnit GetStreet(int id)
        {
            return _context.Streets.Where(item => item.Id == id).FirstOrDefault();
        }
        public StreetUnit GetStreet(string name)
        {
            return _context.Streets.Where(item => item.Name == name).FirstOrDefault();
        }

        public IEnumerable<StreetUnit> GetStreets()
        {
            return _context.Streets.ToList();//.Take(10)
        }

        public bool StreetExists(int id)
        {
            return _context.Streets.Any(item => item.Id == id);
        }
        #endregion

        #region 社区
        public Community GetCommunity(string name)
        {
            return _context.Communitys.Where(item => item.Name == name || item.Alias.Contains(name)).FirstOrDefault();
        }
        #endregion


        #region 网格
        public IQueryable<NetGrid> GetNetGridInCommunity(int id)
        {
            return _context.NetGrids.Where(item => item.Community != null && item.Community.Id == id);
        }
        #endregion

        #region building by网格
        public IQueryable<Building> GetBuildingInNetGrid(int netid)
        {
            return _context.Buildings.Where(item => item.NetGrid != null && item.NetGrid.Id == netid);
        }
        #endregion

        #region 小区
        public Subdivision GetSubdivision(string name)
        {
            return _context.Subdivisions.Where(item => item.Name == name || item.Alias.Contains(name)).FirstOrDefault();
        }
        #endregion


        #region building
        //public Building GetBuildingInCommunity(string commnunityName, string name)
        //{
        //Community com = GetCommunity(commnunityName);
        //if (com != null)
        //    return _context.Buildings.Where(item => item.Community == com && item.Name == name).FirstOrDefault();
        //else
        //   return null;
        // }

        public Building GetBuildingInSubdivision(string subdivisionName, string name)
        {
            Subdivision sub = GetSubdivision(subdivisionName);
            if (sub != null)
            {
                return _context.Buildings.Where(item => item.Subdivision.Id == sub.Id && (item.Name == name || item.Alias == name)).FirstOrDefault();
            }
            else
                return null;
        }



        /// <summary>
        /// 获取小区内的楼栋
        /// </summary>
        /// <param name="id">小区 id</param>
        /// <returns></returns>
        public IQueryable<Building> GetBuildingInSubdivision(int id)
        {
            return _context.Buildings.Where(item => item.Subdivision != null && item.Subdivision.Id == id);
        }
        #endregion

        #region Room
        public Room GetRoom(string subdivisionName, string buidlingName, string roomNO)
        {
            Building building = GetBuildingInSubdivision(subdivisionName, buidlingName);
            if (building != null)
                return _context.Rooms.SingleOrDefault(item => item.Building.Id == building.Id && item.Name == roomNO);
            else
                return null;
        }
        public Room GetRoom(int buidlingId, string roomNO)
        {
            return _context.Rooms.SingleOrDefault(item => item.Building.Id == buidlingId && item.Name == roomNO);
        }
        #endregion

        #region 人员
        /// <summary>
        /// 获取建筑物的住户信息
        /// </summary>
        /// <param name="id">建筑物 id </param>
        /// <returns></returns>
        public IEnumerable<object> GetPersonsByCommunity(string userName)
        {
            IQueryable<Room> rooms = FilterRoomsByRole(userName);
            if (rooms != null)
            {
                //获取rooms内所有人
                IEnumerable<IntermediatePersonRoom> data = GetroomsWithPersons(rooms);
                return GetPersonsByQueryRoom(data);
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<object> GetPersonsByBuilding(int id)
        {

            try
            {
                //根据 room - person 数据
                var roomsWithPersons = from room in _context.Rooms.Where(r => r.Building.Id == id)
                                       from pr in room.PersonRooms
                                       select new
                                       {
                                           RoomId = room.Id,
                                           RoomNO = room.Name,
                                           BulidingName = room.Building.Name,
                                           BulidingAddress = room.Building.Address, // pr.BulidingName,
                                           SubdivsionName = room.Building.Subdivision.Name,
                                           CommunityName = room.Building.NetGrid.Community.Name,
                                           netGridName = room.Building.NetGrid.Name,
                                           age = pr.Person.Age,
                                           sex = pr.Person.Sex,
                                           pr.PersonId,
                                           pr.Person,
                                           IsOwner = pr.IsOwner ? "是" : "否",
                                           IsHouseholder = pr.IsHouseholder ? "是" : "否",
                                           IsLiveHere = pr.IsLiveHere ? "是" : "否",
                                           pr.RelationWithHouseholder,
                                           pr.LodgingReason,
                                           pr.PopulationCharacter
                                       };              
                var psdata = roomsWithPersons.ToList();

                var data = from pr in psdata
                           join sg in _context.SpecialGroups on pr.PersonId equals sg.PersonId into psg // 根据身份证关联
                           select new
                           {
                               pr.RoomId,
                               pr.RoomNO,
                               pr.CommunityName,
                               pr.netGridName,
                               pr.age,
                               pr.sex,
                               pr.SubdivsionName,
                               pr.BulidingName,
                               pr.BulidingAddress,
                               pr.PersonId,
                               pr.Person,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,                             
                               SpecialGroup = psg, // 特殊人群信息
                               //psg.Type,
                           };

                //var d = data.ToList();// ToLookup(sp => sp.p.PersonId, sp => sp.SpecialGroup);
                //var d1 = data as IEnumerable<object>;
                return data;
            }
            catch (Exception e)
            {
                return null;
            }


        }
        //获取网格的住户信息分析（中文）
        public IEnumerable<object> GetPersonsByNetGrid_ZH(int id)
        {
            try
            {
                //根据 room - person 数据
                var roomsWithPersons = from room in _context.Rooms.Where(r => r.Building.NetGrid.Id == id)
                                       from pr in room.PersonRooms
                                       select new
                                       {
                                           RoomId = room.Id,
                                           RoomNO = room.Name,
                                           BulidingName = room.Building.Name,
                                           SubdivsionName = room.Building.Subdivision.Name,
                                           CommunityName = room.Building.NetGrid.Community.Name,
                                           age = pr.Person.Age,
                                           sex = pr.Person.Sex,
                                           pr.PersonId,
                                           pr.Person,
                                           IsOwner = pr.IsOwner ? "是" : "否",
                                           IsHouseholder = pr.IsHouseholder ? "是" : "否",
                                           IsLiveHere = pr.IsLiveHere ? "是" : "否",
                                           pr.RelationWithHouseholder,
                                           pr.LodgingReason,
                                           pr.PopulationCharacter
                                       };

                var psdata = roomsWithPersons.ToList();

                var data = from pr in psdata
                           join sg in _context.SpecialGroups on pr.PersonId equals sg.PersonId into psg // 根据身份证关联
                           select new
                           {
                               性别 = pr.sex,
                               年龄 = pr.age,
                               房间号 = pr.RoomNO,
                               社区名 = pr.CommunityName,
                               小区名 = pr.SubdivsionName,
                               楼栋名 = pr.BulidingName,
                               产权人 = pr.IsOwner,
                               户主 = pr.IsHouseholder,
                               在此居住 = pr.IsLiveHere,
                               与户主关系 = pr.RelationWithHouseholder,
                               寄住原因 = pr.LodgingReason,
                               人口性质 = pr.PopulationCharacter,
                           };

                return data;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        //获取建筑物的住户信息(中文)
        public IEnumerable<object> GetPersonsByBuilding_ZH(int id)
        {            
            try
            {
                //根据 room - person 数据
                var roomsWithPersons = from room in _context.Rooms.Where(r => r.Building.Id == id)
                                       from pr in room.PersonRooms
                                       select new
                                       {
                                           RoomId = room.Id,
                                           RoomNO = room.Name,
                                           BulidingName = room.Building.Name,
                                           SubdivsionName = room.Building.Subdivision.Name,
                                           CommunityName = room.Building.NetGrid.Community.Name,
                                           age = pr.Person.Age,
                                           sex = pr.Person.Sex,
                                           pr.PersonId,
                                           pr.Person,
                                           IsOwner = pr.IsOwner ? "是" : "否",
                                           IsHouseholder = pr.IsHouseholder ? "是" : "否",
                                           IsLiveHere = pr.IsLiveHere ? "是" : "否",
                                           pr.RelationWithHouseholder,
                                           pr.LodgingReason,
                                           pr.PopulationCharacter
                                       };

                var psdata = roomsWithPersons.ToList();

                var data = from pr in psdata
                           join sg in _context.SpecialGroups on pr.PersonId equals sg.PersonId into psg // 根据身份证关联
                           select new
                           {
                               性别 = pr.sex,
                               年龄 = pr.age,
                               房间号 = pr.RoomNO,
                               社区名 = pr.CommunityName,
                               小区名 = pr.SubdivsionName,
                               楼栋名 = pr.BulidingName,
                               产权人 = pr.IsOwner,
                               户主 = pr.IsHouseholder,
                               在此居住 = pr.IsLiveHere,
                               与户主关系 = pr.RelationWithHouseholder,
                               寄住原因 = pr.LodgingReason,
                               人口性质 = pr.PopulationCharacter,
                           };

                return data;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        ///<summary>
        ///
        /// 根据网格获得人员信息表
        /// 
        ///</summary>
        public IEnumerable<object> GetPersonsByNetGrid(int id)
        {
            try
            {
                //根据 room - person 数据
                var roomsWithPersons = from room in _context.Rooms.Where(r => r.Building.NetGrid.Id == id)
                                       from pr in room.PersonRooms
                                       select new
                                       {
                                           RoomId = room.Id,
                                           RoomNO = room.Name,
                                           BulidingName = pr.Room.Building.Name,
                                           BulidingAddress = room.Building.Address, // pr.BulidingAddress,
                                           SubdivsionName = pr.Room.Building.Subdivision.Name,
                                           CommunityName = pr.Room.Building.NetGrid.Community.Name,
                                           netGridName = room.Building.NetGrid.Name,
                                           age = pr.Person.Age,
                                           sex = pr.Person.Sex,
                                           pr.PersonId,
                                           pr.Person,
                                           IsOwner = pr.IsOwner ? "是" : "否",
                                           IsHouseholder = pr.IsHouseholder ? "是" : "否",
                                           IsLiveHere = pr.IsLiveHere ? "是" : "否",
                                           pr.RelationWithHouseholder,
                                           pr.LodgingReason,
                                           pr.PopulationCharacter
                                       };

                var psdata = roomsWithPersons.ToList();

                var data = from pr in psdata
                           join sg in _context.SpecialGroups on pr.PersonId equals sg.PersonId into psg // 根据身份证关联
                           select new
                           {
                               pr.RoomId,
                               pr.RoomNO,
                               pr.CommunityName,
                               pr.netGridName,
                               pr.age,
                               pr.sex,
                               pr.SubdivsionName,
                               pr.BulidingName,
                               pr.BulidingAddress,
                               pr.PersonId,
                               pr.Person,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,
                               SpecialGroup = psg // 特殊人群信息
                           };         
                return data;
            }
            catch (Exception e)
            {
                return null;
            }


        }

        ///<summary>
        ///
        /// 根据楼栋获得人员信息表
        /// 
        ///</summary>
        public IEnumerable<object> GetPersonsBySubdivision(int id)
        {
            try
            {
                //根据 room - person 数据
                var roomsWithPersons = from room in _context.Rooms.Where(r => r.Building.Subdivision.Id == id)
                                       from pr in room.PersonRooms
                                       select new
                                       {
                                           RoomId = room.Id,
                                           RoomNO = room.Name,
                                           BulidingName = pr.Room.Building.Name,
                                           BulidingAddress = room.Building.Address, // pr.BulidingAddress,
                                           SubdivsionName = pr.Room.Building.Subdivision.Name,
                                           CommunityName = pr.Room.Building.NetGrid.Community.Name,
                                           netGridName = room.Building.NetGrid.Name,
                                           age = pr.Person.Age,
                                           sex = pr.Person.Sex,
                                           pr.PersonId,
                                           pr.Person,
                                           IsOwner = pr.IsOwner ? "是" : "否",
                                           IsHouseholder = pr.IsHouseholder ? "是" : "否",
                                           IsLiveHere = pr.IsLiveHere ? "是" : "否",
                                           pr.RelationWithHouseholder,
                                           pr.LodgingReason,
                                           pr.PopulationCharacter
                                       };

                var psdata = roomsWithPersons.ToList();

                var data = from pr in psdata
                           join sg in _context.SpecialGroups on pr.PersonId equals sg.PersonId into psg // 根据身份证关联
                           select new
                           {
                               pr.RoomId,
                               pr.RoomNO,
                               pr.CommunityName,
                               pr.netGridName,
                               pr.age,
                               pr.sex,
                               pr.SubdivsionName,
                               pr.BulidingName,
                               pr.BulidingAddress,
                               pr.PersonId,
                               pr.Person,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,
                               SpecialGroup = psg // 特殊人群信息
                           };
                return data;
            }
            catch (Exception e)
            {
                return null;
            }


        }


        //获取特殊人群的位置信息信息(中文)
        public IEnumerable<object> GetSpecialPersonLoction_ZH()
        {
            try
            {
                var data = from sg in _context.SpecialGroups
                           join p in _context.Persons on sg.PersonId equals p.PersonId
                           from pr in p.PersonRooms
                           select new
                           {
                               姓名 = p.Name,
                               性别 = p.Sex,
                               联系电话 = p.Phone,
                               户籍地 = p.DomicileAddress,
                               居住地址 = pr.Room.Building.Name + '-' + pr.Room.Name,
                               身份证号码 = pr.PersonId,
                               经度 = pr.Room.Longitude,
                               纬度 = pr.Room.Latitude,
                               楼层 = pr.Room.Name.Substring(2, 1),
                               年龄 = p.Age,
                               社区名 = pr.Room.Building.NetGrid.Community.Name,
                               小区名 = pr.Room.Building.Subdivision.Name,
                               类型 = sg.Type,

                           };
                var psdata = data.ToList();
                return psdata;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        /// <summary>
        /// 根据房间号，获取住房人员信息列表
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public IQueryable<Person> GetPersonsInRoom(int roomId)
        {
            List<int> personIDs = _context.PersonRooms.Where(item => item.Room.Id == roomId).Select(item => item.Person.Id).ToList();
            return _context.Persons.Where(item => personIDs.Contains(item.Id));
        }

        /// <summary>
        /// 通楼栋地址 、名、房号，获取住房人信息
        /// </summary>
        /// <param name="commnunityName"></param>
        /// <param name="buidlingName"></param>
        /// <param name="roomNO"></param>
        /// <returns></returns>
        public IQueryable<Person> GetPersonsInRoom(string netName, string addressName, string buidlingName, string roomNO)
        {
            Room room = _context.Rooms.FirstOrDefault(item =>item.Building.NetGrid.Name == netName && item.Building.Address == addressName && item.Building.Name == buidlingName && item.Name == roomNO);

            if (room != null)
            {
                List<int> personIDs = _context.PersonRooms.Where(item => item.Room == room).Select(item => item.Person.Id).ToList();
                return _context.Persons.Where(item => personIDs.Contains(item.Id));
            }
            else
                return null;
        }
        #endregion

        #region 公共服务

        //获取特殊人群类型
        public IQueryable<object> GetSpecialGroupsType()
        {
            var SpecialGroupsType = from sgt in _context.SpecialGroups
                                    select new
                                    {
                                        sgt.Type,
                                    };

            return SpecialGroupsType.Distinct();

        }
        //按权限获取特殊人群
        public IQueryable<object> GetSpecialGroups(string userName)
        {
            try
            {
                var data = from sg in _context.SpecialGroups
                           join p in _context.Persons on sg.PersonId equals p.PersonId
                           from pr in p.PersonRooms
                           select new
                           {
                               RoomId = pr.Room.Id,
                               RoomNO = pr.Room.Name,
                               netid = pr.Room.Building.NetGrid.Id,
                               CommunityName = pr.Room.Building.NetGrid.Community.Name,
                               netGridName = pr.Room.Building.NetGrid.Name,
                               age = p.Age,
                               sex = p.Sex,
                               SubdivsionName = pr.Room.Building.Subdivision.Name,
                               BulidingName = pr.Room.Building.Name,
                               BulidingAddress = pr.Room.Building.Address,
                               p.PersonId,
                               person = p,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,
                               sg.Type

                           };
                var roleName = GetRoleName(userName);
                if (roleName == "网格员")
                {
                    NetGrid netgrid = _context.NetGrids.SingleOrDefault(n => n.User.UserName == userName);
                    var netidUser = netgrid.Id;
                    //网格
                    return data = data.Where(r => r.netid == netidUser);
                }
                else
                if (roleName == "社区")
                {
                    Community community = _context.Communitys.SingleOrDefault(c => c.Alias == userName);
                    var communityName = community.Name;
                    //社区
                    return data = data.Where(r => r.CommunityName == communityName);
                }
                else
                {
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }


        }

        //获取低保户类型
        public IQueryable<object> GetPoorType()
        {
            var poorType = from sgt in _context.PoorPeoples
                                    select new
                                    {
                                        sgt.Type,
                                    };

            return poorType.Distinct();

        }
        //按权限获取低保户
        public IQueryable<object> GetPoorpeople(string userName)
        {
            try
            {
                var data = from sg in _context.PoorPeoples
                           join p in _context.Persons on sg.PersonId equals p.PersonId
                           from pr in p.PersonRooms
                           select new
                           {
                               RoomId = pr.Room.Id,
                               RoomNO = pr.Room.Name,
                               netid = pr.Room.Building.NetGrid.Id,
                               CommunityName = pr.Room.Building.NetGrid.Community.Name,
                               netGridName = pr.Room.Building.NetGrid.Name,
                               age = p.Age,
                               sex = p.Sex,
                               SubdivsionName = pr.Room.Building.Subdivision.Name,
                               BulidingName = pr.Room.Building.Name,
                               BulidingAddress = pr.Room.Building.Address,
                               p.PersonId,
                               person = p,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,
                               sg.Type

                           };
                var roleName = GetRoleName(userName);
                if (roleName == "网格员")
                {
                    NetGrid netgrid = _context.NetGrids.SingleOrDefault(n => n.User.UserName == userName);
                    var netidUser = netgrid.Id;
                    //网格
                    return data = data.Where(r => r.netid == netidUser);
                }
                else
                if (roleName == "社区")
                {
                    Community community = _context.Communitys.SingleOrDefault(c => c.Alias == userName);
                    var communityName = community.Name;
                    //社区
                    return data = data.Where(r => r.CommunityName == communityName);
                }
                else
                {
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }


        }

        //获取残疾人类型和级别
        public IQueryable<object> GetDisabilityType()
        {
            var poorType = from sgt in _context.Disability
                           select new
                           {
                               sgt.Type,
                           };

            return poorType.Distinct();

        }
        public IQueryable<object> GetDisabilitylevel()
        {
            var level = from sgt in _context.Disability
                           select new
                           {
                               sgt.Class,
                           };

            return level.Distinct();

        }
        //按权限获取残疾人信息
        public IQueryable<object> GetDisability(string userName)
        {
            try
            {
                var data = from sg in _context.Disability
                           join p in _context.Persons on sg.PersonId equals p.PersonId
                           from pr in p.PersonRooms
                           select new
                           {
                               RoomId = pr.Room.Id,
                               RoomNO = pr.Room.Name,
                               netid = pr.Room.Building.NetGrid.Id,
                               CommunityName = pr.Room.Building.NetGrid.Community.Name,
                               netGridName = pr.Room.Building.NetGrid.Name,
                               age = p.Age,
                               sex = p.Sex,
                               SubdivsionName = pr.Room.Building.Subdivision.Name,
                               BulidingName = pr.Room.Building.Name,
                               BulidingAddress = pr.Room.Building.Address,
                               p.PersonId,
                               person = p,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,
                               sg.Type,
                               level = sg.Class,

                           };
                var roleName = GetRoleName(userName);
                if (roleName == "网格员")
                {
                    NetGrid netgrid = _context.NetGrids.SingleOrDefault(n => n.User.UserName == userName);
                    var netidUser = netgrid.Id;
                    //网格
                    return data = data.Where(r => r.netid == netidUser);
                }
                else
                if (roleName == "社区")
                {
                    Community community = _context.Communitys.SingleOrDefault(c => c.Alias == userName);
                    var communityName = community.Name;
                    //社区
                    return data = data.Where(r => r.CommunityName == communityName);
                }
                else
                {
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }


        //获取退伍军人类型
        public IQueryable<object> GetMilitaryType()
        {
            var poorType = from sgt in _context.MilitaryService
                           select new
                           {
                               sgt.Type,
                           };

            return poorType.Distinct();

        }
        //按权限获取退伍军人
        public IQueryable<object> GetMilitaryService(string userName)
        {
            try
            {
                var data = from sg in _context.MilitaryService
                           join p in _context.Persons on sg.PersonId equals p.PersonId
                           from pr in p.PersonRooms
                           select new
                           {
                               RoomId = pr.Room.Id,
                               RoomNO = pr.Room.Name,
                               netid = pr.Room.Building.NetGrid.Id,
                               CommunityName = pr.Room.Building.NetGrid.Community.Name,
                               netGridName = pr.Room.Building.NetGrid.Name,
                               age = p.Age,
                               sex = p.Sex,
                               SubdivsionName = pr.Room.Building.Subdivision.Name,
                               BulidingName = pr.Room.Building.Name,
                               BulidingAddress = pr.Room.Building.Address,
                               p.PersonId,
                               person = p,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,
                               sg.Type,

                           };
                var roleName = GetRoleName(userName);
                if (roleName == "网格员")
                {
                    NetGrid netgrid = _context.NetGrids.SingleOrDefault(n => n.User.UserName == userName);
                    var netidUser = netgrid.Id;
                    //网格
                    return data = data.Where(r => r.netid == netidUser);
                }
                else
                if (roleName == "社区")
                {
                    Community community = _context.Communitys.SingleOrDefault(c => c.Alias == userName);
                    var communityName = community.Name;
                    //社区
                    return data = data.Where(r => r.CommunityName == communityName);
                }
                else
                {
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region 用户、角色

        public User GetUserByName(string name)
        {
            var user = _context.Users.Include(u => u.RoleUsers).ThenInclude(ru => ru.Role).SingleOrDefault(r => r.UserName.Equals(name));
            //加载role信息
            //_context.Entry(user).Collection(u => u.RoleUsers).Load();
            return user;
        }
        //根据userName，得RoleName
        public string GetRoleName(string userName)
        {
            var user = GetUserByName(userName);
            var roleList = user.Roles;
            var roleName = roleList[0].Name;
            return roleName;
        }
        //根据alias（社区名称英文首字母缩写）返回社区中文名称
        public string GetCommunityNameByAlias(string alias)
        {
            Community community = _context.Communitys.SingleOrDefault(c => c.Alias == alias);
            return community.Name;
        }
        
        //根据user，返回对应的小区
        public IQueryable<object> GetSubdivsionsByUser(string userName)
        {
            var roleName = GetRoleName(userName);
            if (roleName == "网格员")
            {
                return (from useRow in _context.Users.Where(u => u.UserName == userName)
                        from grid in useRow.NetGrid
                        select new
                        {
                            grid.Community.Subdivisions[0].Id,
                            grid.Community.Subdivisions[0].Name
                        });
            
            }

            if (roleName == "社区")
            {
                var communityName = GetCommunityNameByAlias(userName);
                return (from comm in _context.Communitys.Where(u => u.Name == communityName)
                        select new
                        {
                            comm.Subdivisions[0].Id,
                            comm.Subdivisions[0].Name

                        });
            }

            return _context.Subdivisions;
        }

        //根据网格员，返回相应小区
        public IQueryable<object> GetSubdivisionByNetGrid(string userName)
        {
            return from u in _context.Users.Where(u => u.UserName == userName)
                   from ng in u.NetGrid
                   from b in ng.Buildings
                   select new
                   {
                       id = b.Subdivision.Id,
                       subdivisionName = b.Subdivision.Name,
                   };
        }

        //根据网格员，返回相应楼栋
        public IQueryable<object> GetBuildingsByNetGrid(string userName)
        {
            return from u in _context.Users.Where(u => u.UserName == userName)
                   from ng in u.NetGrid
                   from b in ng.Buildings
                   select new
                   {
                       b.Id,
                       buildingName = b.Name,
                       b.Address
                   };
        }
        //根据楼栋，返回房屋
        public IQueryable<object> GetRoomsByBuilding(string buildingName, string address)
        {
            return from b in _context.Buildings.Where(b => b.Name == buildingName && b.Address == address)
                   from r in b.Rooms
                   select new
                   {
                       r.Id,
                       RoomName = r.Name,
                       r.Category,
                       r.Use,
                       r.Area,
                       r.Longitude,
                       r.Latitude,
                       r.Height,
                       NetGridName = b.NetGrid.Name,
                       CommunityName = b.NetGrid.Community.Name,
                       b.Address,
                       BuildingName = b.Name,
                   };
        }

        //根据楼栋，返回人房数据
        public IQueryable<object> GetPersonRoomsByUser_WithBuinldingAndRoom(string userName, string buildingName, string roomName)
        {
            return from u in _context.Users.Where(u => u.UserName == userName)
                   from ng in u.NetGrid
                   from b in ng.Buildings.Where(b => b.Name == buildingName)
                   from room in b.Rooms.Where(r => r.Name == roomName || roomName == "")
                   from pr in room.PersonRooms
                   select new
                   {
                       pr.Id,
                       pr.Person.PersonId,
                       pr.Person.Name,
                       pr.Person.EthnicGroups,
                       pr.Person.Phone,
                       pr.Person.DomicileAddress,
                       pr.Person.Company,
                       pr.Person.PoliticalState,
                       pr.Person.OrganizationalRelation,
                       IsOverseasChinese = pr.Person.IsOverseasChinese ? "是" : "否",
                       pr.Person.MerriedStatus,
                       pr.Person.Note,
                       pr.Status,
                       IsHouseholder = pr.IsHouseholder ? "是" : "否",
                       pr.RelationWithHouseholder,
                       IsOwner = pr.IsOwner ? "是" : "否",
                       IsLiveHere = pr.IsLiveHere ? "是" : "否",
                       pr.PopulationCharacter,
                       pr.LodgingReason,
                       RoomName = room.Name,
                       RoomUse = room.Use,
                       room.Category,
                       room.Area,
                       room.Other,
                       roomNote = room.Note,
                       SubdivisionName = b.Subdivision.Name,
                       b.Address,
                       BuildingName = b.Name,
                       NetGrid = ng.Name,
                       CommunityName = ng.Community.Name,
                       StreetName = ng.Community.Street.Name,
                   };

        }
       

        public IEnumerable<object> GetPersonsByUser(string userName){
            var roleName = GetRoleName(userName);

            //网格
            if (roleName == "网格员")
            {
                var personHouseInfo = from u in _context.Users.Where(u => u.UserName == userName)
                                 from ng in u.NetGrid
                                 from b in ng.Buildings
                                 from room in b.Rooms
                                 from pr in room.PersonRooms
                                 select new
                                 {
                                    pr.Id,
                                    pr.Person.PersonId,
                                    pr.Person.Name,
                                    pr.Person.EthnicGroups,
                                    pr.Person.Phone,
                                    pr.Person.DomicileAddress,
                                    pr.Person.Company,
                                    pr.Person.PoliticalState,
                                    pr.Person.OrganizationalRelation,
                                    IsOverseasChinese = pr.Person.IsOverseasChinese ? "是" : "否",
                                    pr.Person.MerriedStatus,
                                    pr.Person.Note,
                                    pr.Status,
                                    IsHouseholder = pr.IsHouseholder ? "是" : "否",
                                    pr.RelationWithHouseholder,
                                    IsOwner = pr.IsOwner ? "是": "否",
                                    IsLiveHere = pr.IsLiveHere ? "是" : "否",
                                    pr.PopulationCharacter,
                                    pr.LodgingReason,
                                    RoomName = room.Name,
                                    RoomUse = room.Use,
                                    room.Category,
                                    room.Area,
                                    room.Other,
                                    RoomNote = room.Note,
                                    SubdivisionName = b.Subdivision.Name,
                                    b.Address,
                                    BuildingName = b.Name,
                                    NetGrid = ng.Name,
                                    CommunityName = ng.Community.Name,
                                    StreetName = ng.Community.Street.Name,
                                 };

                var personHouseEditInfo = from phei in _context.PersonHouseDatas.Where(phd => phd.Status != "rejected" && phd.Status != "approved" && phd.Operation == "creating" && phd.Editor == userName )
                                          select new
                                          {
                                              phei.Id,
                                              phei.PersonId,
                                              phei.Name,
                                              phei.EthnicGroups,
                                              phei.Phone,
                                              phei.DomicileAddress,
                                              phei.Company,
                                              phei.PoliticalState,
                                              phei.OrganizationalRelation,
                                              IsOverseasChinese = phei.IsOverseasChinese ? "是" : "否",
                                              phei.MerriedStatus,
                                              phei.Note,
                                              phei.Status,
                                              IsHouseholder = phei.IsHouseholder ? "是" : "否",
                                              phei.RelationWithHouseholder,
                                              IsOwner = phei.IsOwner ? "是" : "否",
                                              IsLiveHere = phei.IsLiveHere ? "是" : "否",
                                              phei.PopulationCharacter,
                                              phei.LodgingReason,
                                              phei.RoomName,
                                              phei.RoomUse,
                                              phei.Category,
                                              phei.Area,
                                              phei.Other,
                                              phei.RoomNote,
                                              phei.SubdivisionName,
                                              phei.Address,
                                              phei.BuildingName,
                                              phei.NetGrid,
                                              phei.CommunityName,
                                              phei.StreetName
                                          };

                     return personHouseInfo.AsEnumerable().Union(personHouseEditInfo.AsEnumerable());

            }

            //社区
            if (roleName == "社区")
            {
                var communityName = GetCommunityNameByAlias(userName);
                return SearchPersonHouseInfo(communityName, null);

            }

            //街道
            return SearchPersonHouseInfo( null, null);
        }


        //返回指定网格员编辑的数据
        public IQueryable<object> SearchPersonHouseByNetGrid(string editor)
        {
            return from phei in _context.PersonHouseDatas.Where(phd => phd.Status != "approved" && phd.Editor == editor)
                   select new
                   {
                       phei.Id,
                       phei.PersonId,
                       phei.Name,
                       phei.EthnicGroups,
                       phei.Phone,
                       phei.DomicileAddress,
                       phei.Company,
                       phei.PoliticalState,
                       phei.OrganizationalRelation,
                       IsOverseasChinese = phei.IsOverseasChinese ? "是" : "否",
                       phei.MerriedStatus,
                       phei.Note,
                       phei.Status,
                       IsHouseholder = phei.IsHouseholder ? "是" : "否",
                       phei.RelationWithHouseholder,
                       IsOwner = phei.IsOwner ? "是" : "否",
                       IsLiveHere = phei.IsLiveHere ? "是" : "否",
                       phei.PopulationCharacter,
                       phei.LodgingReason,
                       phei.RoomName,
                       phei.RoomUse,
                       phei.Category,
                       phei.Area,
                       phei.Other,
                       phei.RoomNote,
                       phei.SubdivisionName,
                       phei.Address,
                       phei.BuildingName,
                       phei.NetGrid,
                       phei.CommunityName,
                       phei.StreetName,
                       phei.Editor,
                       phei.EditTime,
                       phei.Operation,
                   };
        }
        //根据用户，返回personHouse数据
        public IQueryable<object> SearchPersonHouseInfo (string communityName, string netGridName)
        {
            if(netGridName != null)
            {
                return SearchPersonHouseByNetGrid(netGridName);
            }
            if (communityName != null)
            {
                return from phei in _context.PersonHouseDatas.Where(phd => phd.Status != "approved" && phd.CommunityName == communityName)
                        select new
                        {
                            phei.Id,
                            phei.PersonId,
                            phei.Name,
                            phei.EthnicGroups,
                            phei.Phone,
                            phei.DomicileAddress,
                            phei.Company,
                            phei.PoliticalState,
                            phei.OrganizationalRelation,
                            IsOverseasChinese = phei.IsOverseasChinese ? "是" : "否",
                            phei.MerriedStatus,
                            phei.Note,
                            phei.Status,
                            IsHouseholder = phei.IsHouseholder ? "是" : "否",
                            phei.RelationWithHouseholder,
                            IsOwner = phei.IsOwner ? "是" : "否",
                            IsLiveHere = phei.IsLiveHere ? "是" : "否",
                            phei.PopulationCharacter,
                            phei.LodgingReason,
                            phei.RoomName,
                            phei.RoomUse,
                            phei.Category,
                            phei.Area,
                            phei.Other,
                            phei.RoomNote,
                            phei.SubdivisionName,
                            phei.Address,
                            phei.BuildingName,
                            phei.NetGrid,
                            phei.CommunityName,
                            phei.StreetName,
                            phei.Editor,
                            phei.EditTime,
                            phei.Operation,
                        };
            }
            //若未指定社区或网格，则返回街道数据
            return from phei in _context.PersonHouseDatas.Where(phd => phd.Status == "approved" || phd.Status == "rejected" || phd.Status == "verified")
                    select new
                    {
                        phei.Id,
                        phei.PersonId,
                        phei.Name,
                        phei.EthnicGroups,
                        phei.Phone,
                        phei.DomicileAddress,
                        phei.Company,
                        phei.PoliticalState,
                        phei.OrganizationalRelation,
                        IsOverseasChinese = phei.IsOverseasChinese ? "是" : "否",
                        phei.MerriedStatus,
                        phei.Note,
                        phei.Status,
                        IsHouseholder = phei.IsHouseholder ? "是" : "否",
                        phei.RelationWithHouseholder,
                        IsOwner = phei.IsOwner ? "是" : "否",
                        IsLiveHere = phei.IsLiveHere ? "是" : "否",
                        phei.PopulationCharacter,
                        phei.LodgingReason,
                        phei.RoomName,
                        phei.RoomUse,
                        phei.Category,
                        phei.Area,
                        phei.Other,
                        phei.RoomNote,
                        phei.SubdivisionName,
                        phei.Address,
                        phei.BuildingName,
                        phei.NetGrid,
                        phei.CommunityName,
                        phei.StreetName,
                        phei.Editor,
                        phei.EditTime,
                        phei.Operation,
                    };
        }

        //创建一条personHouse
        public PersonHouseData CreatePersonHouse(string userName, PersonUpdateParamTesting personFields)
        {
           return new PersonHouseData
                    {
                        PersonId = personFields.PersonId,
                        Name = personFields.Name,
                        EthnicGroups = personFields.EthnicGroups,
                        Phone = personFields.Phone,
                        DomicileAddress = personFields.DomicileAddress,
                        Company = personFields.Company,
                        PoliticalState = personFields.PoliticalState,
                        OrganizationalRelation = personFields.OrganizationalRelation,
                        IsOverseasChinese = personFields.IsOverseasChinese == "是" ? true : false,
                        MerriedStatus = personFields.MerriedStatus,
                        Note = personFields.Note,
                        Status = personFields.Status,
                        IsHouseholder = personFields.IsHouseholder == "是" ? true : false,
                        RelationWithHouseholder = personFields.RelationWithHouseholder,
                        IsOwner = personFields.IsOwner == "是" ? true : false,
                        IsLiveHere = personFields.IsLiveHere == "是" ? true : false,
                        PopulationCharacter = personFields.PopulationCharacter,
                        LodgingReason = personFields.LodgingReason,
                        Category = personFields.Category,
                        RoomName = personFields.RoomName,
                        RoomUse = personFields.RoomUse,
                        BuildingName = personFields.BuildingName,
                        Address = personFields.Address,
                        SubdivisionName = personFields.SubdivisionName,
                        NetGrid = personFields.NetGrid,
                        CommunityName = personFields.CommunityName,
                        Editor = userName,
                        EditTime = DateTime.Now.ToString(),
                        Operation = personFields.Operation
                    };
        }
        //修改一条personHouse
        public void UpdatePersonHouse(string userName,PersonHouseData targetPersonHouse, PersonUpdateParamTesting personFields)
        {
            targetPersonHouse.PersonId = personFields.PersonId;
            targetPersonHouse.Name = personFields.Name;
            targetPersonHouse.EthnicGroups = personFields.EthnicGroups;
            targetPersonHouse.Phone = personFields.Phone;
            targetPersonHouse.DomicileAddress = personFields.DomicileAddress;
            targetPersonHouse.Company = personFields.Company;
            targetPersonHouse.PoliticalState = personFields.PoliticalState;
            targetPersonHouse.OrganizationalRelation = personFields.OrganizationalRelation;
            targetPersonHouse.IsOverseasChinese = personFields.IsOverseasChinese == "是" ? true : false;
            targetPersonHouse.MerriedStatus = personFields.MerriedStatus;
            targetPersonHouse.Note = personFields.Note;
            targetPersonHouse.Status = personFields.Status;
            targetPersonHouse.IsHouseholder = personFields.IsHouseholder == "是" ? true : false;
            targetPersonHouse.RelationWithHouseholder = personFields.RelationWithHouseholder;
            targetPersonHouse.IsOwner = personFields.IsOwner == "是" ? true : false;
            targetPersonHouse.IsLiveHere = personFields.IsLiveHere == "是" ? true : false; 
            targetPersonHouse.PopulationCharacter = personFields.PopulationCharacter;
            targetPersonHouse.LodgingReason = personFields.LodgingReason;
            targetPersonHouse.RoomName = personFields.RoomName;
            targetPersonHouse.RoomUse = personFields.RoomUse;
            targetPersonHouse.Category = personFields.Category;
            targetPersonHouse.BuildingName = personFields.BuildingName;
            targetPersonHouse.Address = personFields.Address;
            targetPersonHouse.NetGrid = personFields.NetGrid;
            targetPersonHouse.CommunityName = personFields.CommunityName;
            targetPersonHouse.Editor = userName;
            targetPersonHouse.EditTime = DateTime.Now.ToString();
        }
        //选中一条personRoom，并修改状态值
        public void UpdatePersonRoomStatus(PersonUpdateParamTesting personFields)
        {
            var personRoom = _context.PersonRooms.SingleOrDefault(pr => pr.PersonId == personFields.PersonId
                                                                          && pr.Room.Name == personFields.RoomName
                                                                          && pr.Room.Building.Name == personFields.BuildingName
                                                                          && pr.Room.Building.Address == personFields.Address );

            personRoom.Status = personFields.Status;
        }
       
        //网格员提交新增、修改、删除指定的人房信息
        public void UpdatePersonHouseByNetGrid(string userName, PersonUpdateParamTesting personFields)
        {
            Room targetRoom = PickRoom(personFields.RoomName, personFields.BuildingName, personFields.Address);
            PersonHouseData targetPersonHouse = PickPersonHouse(personFields.PersonId, personFields.RoomName, personFields.BuildingName, personFields.Address) ;
              
            if(targetRoom != null)
            {
                if (targetPersonHouse == null)
                {
                    targetPersonHouse = CreatePersonHouse(userName, personFields);
                    if (personFields.Operation != "creating")
                    {
                        //修改或删除操作是对原有数据进行操作的，设定在对应的personRoom信息条中标明操作状态
                        UpdatePersonRoomStatus(personFields);
                    }
                    _context.PersonHouseDatas.Add(targetPersonHouse);

                }
                else
                {
                    if (targetPersonHouse.Operation == "creating" && personFields.Operation == "updating")
                    {
                        UpdatePersonHouse(userName, targetPersonHouse, personFields);
                    }
                    else if (targetPersonHouse.Operation == "creating" && personFields.Operation == "deleting")
                    {
                        _context.PersonHouseDatas.Remove(targetPersonHouse);
                    }

                    if (targetPersonHouse.Operation == personFields.Operation && personFields.Operation != "creating")//只允许同为updating或deleting两种可能
                    {
                        UpdatePersonHouse(userName, targetPersonHouse, personFields);
                        UpdatePersonRoomStatus(personFields);
                    }
                    else if ((targetPersonHouse.Operation == "deleting" && personFields.Operation == "updating") || (targetPersonHouse.Operation == "updating" && personFields.Operation == "deleting"))
                    {
                        UpdatePersonHouse(userName, targetPersonHouse, personFields);

                        targetPersonHouse.Operation = personFields.Operation;//对原有数据进行修改转删除或删除转修改，需改变操作类型。

                        UpdatePersonRoomStatus(personFields);
                    }
                }

                _context.SaveChanges();
            }          

            //return GetPersonsByUser(userName);

        }
        public List<object> CreatePersonHouse_batching(string userName, List<PersonUpdateParamTesting> PersonHouseDatas)
        {
            var dataList = new List<object> { };
            bool latchValue = true;
            try
            {
                for (int i = 0; i < PersonHouseDatas.Count; i++)
                {
                    var personHouse = PersonHouseDatas[i];
                    NetGrid resNetGrid = PickNetGrid(personHouse.NetGrid, personHouse.CommunityName);
                    //bool res = GetUserNameByNetGrid(userName , personHouse.NetGrid, personHouse.CommunityName);
                    Room resRoom = PickRoom(personHouse.RoomName, personHouse.BuildingName, personHouse.Address);
                    if (!(resNetGrid != null && resRoom != null))
                    {
                        latchValue = false;
                        dataList.Add(new
                        {
                            message = "房屋地址不正确(填写错误或超出权限)",
                            index = i
                        });
                    }
                }
                if (!latchValue)
                {
                    return dataList;//中止函数执行，并返回错误消息数组
                }

                for (int j = 0; j < PersonHouseDatas.Count; j++)
                {
                    var personHouseItem = PersonHouseDatas[j];

                    PersonHouseData targetPersonHouse = PickPersonHouse(personHouseItem.PersonId, personHouseItem.RoomName, personHouseItem.BuildingName, personHouseItem.Address);
                    if (targetPersonHouse == null)
                    {
                        targetPersonHouse = CreatePersonHouse(userName, personHouseItem);
                        _context.PersonHouseDatas.Add(targetPersonHouse);
                        _context.SaveChanges();
                        dataList.Add(new
                        {
                            message = "ok",
                            index = j
                        });
                    }
                    else
                    {
                        dataList.Add(new
                        {
                            message = "已存在",
                            index = j
                        });
                    }
                }
                return dataList;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        //选中一条personHouseData数据
        public PersonHouseData PickPersonHouse(string PersonId, string RoomName,string BuildingName, string Address)
        {
            return _context.PersonHouseDatas.SingleOrDefault(phd => phd.PersonId == PersonId
                                                                && phd.RoomName == RoomName
                                                                && phd.BuildingName == BuildingName
                                                                && phd.Address == Address );
        }

        public bool GetUserNameByNetGrid(string TarUserName, string NetGrid, string CommunityName)
        {
            var res_query = _context.Users.SingleOrDefault(u => u.UserName == TarUserName);

            List<NetGrid> res = res_query.NetGrid;
            foreach(var item in res)
            {
                var xx = item;
            }
            return true;

        }
       
        //选中一条personRoom数据
        public PersonRoom PickPersonRoom(VerifyAndConfirmParam personHouseFileds)
        {
           return _context.PersonRooms.SingleOrDefault(pr => pr.PersonId == personHouseFileds.PersonId
                                                            && pr.Room.Name == personHouseFileds.RoomName
                                                            && pr.Room.Building.Name == personHouseFileds.BuildingName
                                                            && pr.Room.Building.Address == personHouseFileds.Address );
        }
        //选中一个房间
        public Room PickRoom(string RoomName, string BuildingName, string Address) { 
            return _context.Rooms.SingleOrDefault(r => r.Name == RoomName
                                                    && r.Building.Name == BuildingName
                                                    && r.Building.Address == Address );
        }
        //社区审核网格员的提交
        public void VerifyByCommunity(VerifyAndConfirmParam verifyFileds)
        {
            PersonHouseData personHouse = PickPersonHouse(verifyFileds.PersonId, verifyFileds.RoomName, verifyFileds.BuildingName, verifyFileds.Address);
            PersonRoom personRoom = PickPersonRoom(verifyFileds);
            personHouse.Status = verifyFileds.Status;
            if(personRoom != null)
            {
               personRoom.Status = verifyFileds.Status;
            }

            _context.SaveChanges();
        }

        //街道审批社区的审核
        public IEnumerable<object> ConfirmByAdmin(VerifyAndConfirmParam confirmFields, string userName)
        {
            try
            {
                PersonHouseData personHouse = PickPersonHouse(confirmFields.PersonId, confirmFields.RoomName, confirmFields.BuildingName, confirmFields.Address);
                PersonRoom personRoom = PickPersonRoom(confirmFields);
                personHouse.Status = confirmFields.Status;//personHouseData的状态总会改变

                if (confirmFields.Status == "approved")//街道批准时，执行
                {
                    Person targetPerson = _context.Persons.SingleOrDefault(per => per.PersonId == confirmFields.PersonId);
                    Room targetRoom = PickRoom(confirmFields.RoomName, confirmFields.BuildingName, confirmFields.Address);
                    //新建一条personRoom
                    if (personHouse.Operation == "creating" && personRoom == null && targetRoom != null)
                    {
                        targetRoom.Use = personHouse.RoomUse;//更改房屋用途
                        targetRoom.Category = personHouse.Category;//更改房屋性质(类别)
                                                                   //（以上room属性的修改会影响到关联此room的所有personRoom信息）

                        if (targetPerson == null)
                        {
                            targetPerson = new Person
                            {
                                PersonId = personHouse.PersonId,
                                Name = personHouse.Name,
                                EthnicGroups = personHouse.EthnicGroups,
                                Phone = personHouse.Phone,
                                DomicileAddress = personHouse.DomicileAddress,
                                Company = personHouse.Company,
                                PoliticalState = personHouse.PoliticalState,
                                OrganizationalRelation = personHouse.OrganizationalRelation,
                                IsOverseasChinese = personHouse.IsOverseasChinese,
                                MerriedStatus = personHouse.MerriedStatus,
                                Note = personHouse.Note
                            };
                            _context.Persons.Add(targetPerson);
                        }

                        

                        PersonRoom newPersonRoom = new PersonRoom
                        {
                            PersonId = personHouse.PersonId,
                            IsHouseholder = personHouse.IsHouseholder,
                            RelationWithHouseholder = personHouse.RelationWithHouseholder,
                            IsOwner = personHouse.IsOwner,
                            IsLiveHere = personHouse.IsLiveHere,
                            PopulationCharacter = personHouse.PopulationCharacter,
                            LodgingReason = personHouse.LodgingReason,
                            Status = null
                        };
                        newPersonRoom.Person = targetPerson;//targetPerson为null时，关联新增的person信息；不为null时，关联该targetPerson
                        newPersonRoom.Room = targetRoom;
                        _context.PersonRooms.Add(newPersonRoom);

                    }
                    else if (personHouse.Operation == "updating" && targetPerson != null && targetRoom != null)
                    {
                        targetPerson.Name = personHouse.Name;
                        targetPerson.EthnicGroups = personHouse.EthnicGroups;
                        targetPerson.Phone = personHouse.Phone;
                        targetPerson.DomicileAddress = personHouse.DomicileAddress;
                        targetPerson.Company = personHouse.Company;
                        targetPerson.PoliticalState = personHouse.PoliticalState;
                        targetPerson.OrganizationalRelation = personHouse.OrganizationalRelation;
                        targetPerson.IsOverseasChinese = personHouse.IsOverseasChinese;
                        targetPerson.MerriedStatus = personHouse.MerriedStatus;
                        targetPerson.Note = personHouse.Note;
                        personRoom.IsHouseholder = personHouse.IsHouseholder;
                        personRoom.RelationWithHouseholder = personHouse.RelationWithHouseholder;
                        personRoom.IsOwner = personHouse.IsOwner;
                        personRoom.IsLiveHere = personHouse.IsLiveHere;
                        personRoom.PopulationCharacter = personHouse.PopulationCharacter;
                        personRoom.LodgingReason = personHouse.LodgingReason;
                        personRoom.Status = null;
                        targetRoom.Use = personHouse.RoomUse;
                        targetRoom.Category = personHouse.Category;
                    }
                    else if (personHouse.Operation == "deleting")
                    {
                        _context.PersonRooms.Remove(personRoom);
                        var pr_count = _context.PersonRooms.Where(pr => pr.PersonId == confirmFields.PersonId).ToArray().Length;
                        if (pr_count == 0)
                        {
                            _context.Persons.Remove(targetPerson);
                        }

                    }
                }
                else if (confirmFields.Status == "rejected" && personRoom != null)//街道不批准时，执行
                {
                    personRoom.Status = confirmFields.Status;
                }


                _context.SaveChanges();

                return GetPersonsByUser(userName);
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public IQueryable<object> PersonHouseHistoryInfo()
        {
            return _context.PersonHouseDatas.Where(prd => prd.Status != "approved");
        }

        #endregion

        #region 二维人员查询20210724

        //通过用户名获取社区
        public IQueryable<object> GetCommunitysByUserName(string userName)
        {
            var roleName = GetRoleName(userName);
            if (roleName == "网格员")
            {
                var community = from u in _context.Users.Where(u => u.UserName == userName)
                                from ng in u.NetGrid
                                select new
                                {
                                    id = ng.Community.Id,
                                    Name = ng.Community.Name,
                                };
                return community;

            }
            else
            if (roleName == "社区")
            {   
                return _context.Communitys.Where(c => c.Alias == userName);
            }
            else
            {
                return _context.Communitys;
            }
                        
        }
        //通过用户名获取 网格等
        public IQueryable<NetGrid> GetNetGridByUserName(string userName)
        {
            return _context.NetGrids.Where(n => n.User.UserName == userName);        

        }
        #endregion


        #region 高级检索
        // 获取所有字段
        public IEnumerable<string> GetFields()
        {
            String[] fields = { "社区", "网格","楼栋", "房间", "姓名", "电话", "身份证", "年龄", "性别","民族" };
            return fields;
        }
      

        //高级检索主入口函数
        public IEnumerable<object> GetDataByQuery(string userName, List<string[]> queries)
        {
            var roleName = GetRoleName(userName);
            if(roleName == "Administrator")
            {
                //获取街道级别的room
                var roomsbystreet = _context.Rooms.AsQueryable();
                //根据社区网格楼栋过滤房间
                IQueryable<Room> rroom = FilterRooms(roomsbystreet, queries);
                //通过房间获取room里的人
                IEnumerable<IntermediatePersonRoom> data = GetroomsWithPersons(rroom);
                //根据姓名、电话、身份证、年龄，性别过滤人
                data = FilterPersons(data, queries);
                return GetPersonsByQueryRoom(data);
            }
            else
            {
                //根据用户名获取本辖区的房间
                IQueryable<Room> rooms = FilterRoomsByRole(userName);
                //通过小区楼栋房间等限制条件返回rooms
                rooms = FilterRooms(rooms, queries);
                //获取rooms内所有人
                IEnumerable<IntermediatePersonRoom> data = GetroomsWithPersons(rooms);
                //根据姓名、电话、身份证、年龄，性别过滤人
                data = FilterPersons(data, queries);
                return GetPersonsByQueryRoom(data);
            }
        }

        //第一步：前端传来的过滤条件小区、楼栋、房间
        private IQueryable<Room> FilterRooms (IQueryable<Room> rooms, List<string[]> queries) 
        {
            //var rooms = _context.Rooms.AsQueryable();
            foreach (var query in queries)
            {
                if (query[0] == "社区")
                {
                    rooms = rooms.Where(r => r.Building.NetGrid.Community.Name.Contains(query[2]));
                }
                if (query[0] == "网格")
                {
                    rooms = rooms.Where(r => r.Building.NetGrid.Name.Contains(query[2]));
                }
                if (query[0] == "楼栋")
                {
                    rooms = rooms.Where(r => r.Building.Name.Contains(query[2]) || r.Building.Alias.Contains(query[2]));
                }
                if (query[0] == "房间")
                {
                    rooms = rooms.Where(r => r.Name.Contains(query[2]));
                }
               
            }
            return rooms;
        }

        //第二步：获取过滤后的rooms内的所有人,构造二维数据表
        private IEnumerable<IntermediatePersonRoom> GetroomsWithPersons(IQueryable<Room> rooms)
        {
            try
            {
                var roomsWithPersons = from room in rooms
                                       from pr in room.PersonRooms
                                       select new IntermediatePersonRoom
                                       {
                                           RoomId = room.Id,
                                           RoomNO = room.Name,
                                           BulidingName = room.Building.Name,
                                           BulidingAddress = room.Building.Address, 
                                           SubdivsionName = room.Building.Subdivision.Name,
                                           CommunityName = room.Building.NetGrid.Community.Name,
                                           NetGridName = room.Building.NetGrid.Name,
                                           Sex = pr.Person.Sex,
                                           PersonId = pr.PersonId,
                                           Age = pr.Person.Age,
                                           Person = pr.Person,
                                           IsOwner = pr.IsOwner ? "是" : "否",
                                           IsHouseholder = pr.IsHouseholder ? "是" : "否",
                                           IsLiveHere = pr.IsLiveHere ? "是" : "否",
                                           RelationWithHouseholder = pr.RelationWithHouseholder,
                                           LodgingReason = pr.LodgingReason,
                                           PopulationCharacter = pr.PopulationCharacter
                                       };
                return roomsWithPersons;

            }
            catch (Exception e)
            {
                return null;
            }
        }
        //前端传来的姓名、电话、身份证、年龄过滤人
        private IEnumerable<IntermediatePersonRoom> FilterPersons (IEnumerable<IntermediatePersonRoom> roomsWithPersons , List<string[]> queries)
        {
            foreach (var query in queries)
            {
                if (query[0] == "姓名")
                {
                    roomsWithPersons = roomsWithPersons.Where(pr => pr.Person.Name.Contains(query[2]));
                }
                if (query[0] == "电话")
                {
                    roomsWithPersons = roomsWithPersons.Where(pr => pr.Person.Phone == query[2]);
                }
                if (query[0] == "身份证")
                {
                    roomsWithPersons = roomsWithPersons.Where(pr => pr.Person.PersonId == query[2]);
                }
                if (query[0] == "年龄")
                {
                    try
                    {
                        string[] age = query[2].Split('-');
                        //int start = int.Parse(age[0]);
                        //int end = int.Parse(age[1]);
                        if (query[1] == "介于")
                        {
                            roomsWithPersons = roomsWithPersons.AsEnumerable().Where(pr => pr.Person.Age >= int.Parse(age[0]) && pr.Person.Age <= int.Parse(age[1])).AsQueryable();
                        }
                        if (query[1] == "=")
                        {
                         roomsWithPersons = roomsWithPersons.AsEnumerable().Where(pr => pr.Person.Age == int.Parse(age[0])).AsQueryable();
                        }

                        //.Include(r => r.PersonRooms).ThenInclude(pr => pr.Person)
                        //.Include(r => r.Building).ThenInclude(b => b.Subdivision).ThenInclude(s => s.Community)
                        //.AsEnumerable()
                        //.Where(pr => pr.Person.Age > start && pr.Person.Age < end)).AsQueryable();
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                if (query[0] == "性别")
                {
                    roomsWithPersons = roomsWithPersons.Where(pr => pr.Person.Sex == query[2]);
                }
            }
            return roomsWithPersons;

       }
         private IEnumerable<object> GetPersonsByQueryRoom(IEnumerable<IntermediatePersonRoom> roomsWithPersons) 
        { 
                var psdata = roomsWithPersons.ToList();
                var data = from pr in psdata
                           join sg in _context.SpecialGroups on pr.PersonId equals sg.PersonId into psg // 根据身份证关联
                           select new
                           {
                               pr.RoomId,
                               pr.RoomNO,
                               pr.CommunityName,
                               pr.SubdivsionName,
                               pr.BulidingName,
                               pr.BulidingAddress,
                               pr.PersonId,
                               pr.Age,
                               pr.Sex,
                               pr.NetGridName,
                               pr.Person,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,
                               SpecialGroup = psg // 特殊人群信息
                           };
                return data.Take(20000);
        }
        #endregion

        #region 房屋管理
        //根据单元数、楼层数批量新建房屋
        public IEnumerable<object> BatchingRoomsCreating(RoomCreatingParam batchingParam)
        {
            Building targetBuilding = PickBuilding(batchingParam.BuildingName, batchingParam.Address);
            if(targetBuilding == null)
            {
                targetBuilding = new Building
                {
                    Name = batchingParam.BuildingName,
                    Address = batchingParam.Address
                };
                NetGrid targetNetGrid = PickNetGrid(batchingParam.NetGridName, batchingParam.CommunityName);
                targetBuilding.NetGrid = targetNetGrid;
                _context.Buildings.Add(targetBuilding);
            
                for(int u = 1; u <= batchingParam.Units; u++)
                {
                    for (int f = 1; f <= batchingParam.Floors; f++)
                    {
                        Room Room1 = new Room
                        {
                            Name = $"{u}-{f}01"
                        };
                        Room Room2 = new Room
                        {
                            Name = $"{u}-{f}02"
                        };
                        Room1.Building = targetBuilding;
                        Room2.Building = targetBuilding;
                        _context.Rooms.Add(Room1);
                        _context.Rooms.Add(Room2);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return Enumerable.Empty<object>();
            }

            return GetRooms(batchingParam.BuildingName, batchingParam.NetGridName, batchingParam.CommunityName);
        }
        //根据导入的Excel表格数据批量新建房屋
        public IEnumerable<object> BatchingCreateRoomsWithExcel(List<RoomCreatingParam_Other> RoomsList)
        {
            var dataList = new List<object> { };
            bool latchValue = true;
            for (int i = 0; i < RoomsList.Count; i++)
            {
                var item = RoomsList[i];
                var res = PickNetGrid(item.NetGridName, item.CommunityName);
                if (res == null)
                {
                    latchValue = false;
                    var info = new
                    {
                        index = i,
                        message = "网格不存在"
                    };
                    dataList.Add(info);
                    
                }
            }//检查输入的所有网格是否都存在
            if (!latchValue)
            {
                return dataList;
            }
            else
            {
                foreach (var item in RoomsList)
                {
                    var createdRoom = CreateRoomsWithExcel(item);
                    dataList.Add(createdRoom);
                }
                return dataList;
            }
        }
       
        //新建房屋
        public object CreateRoomsWithExcel(RoomCreatingParam_Other batchingParam)
        {
            Building targetBuilding = PickBuilding(batchingParam.BuildingName, batchingParam.Address);
            NetGrid targetNetGrid = PickNetGrid(batchingParam.NetGridName, batchingParam.CommunityName);
            Room targetRoom = PickRoom(batchingParam.RoomName, batchingParam.BuildingName, batchingParam.Address);
          
            if (targetBuilding == null)//目标楼栋不存在时，判定属于目标楼栋的房屋不可能存在: 新建该房屋
            {
                targetBuilding = new Building
                {
                    Name = batchingParam.BuildingName,
                    Address = batchingParam.Address,
                };
                _context.Buildings.Add(targetBuilding);

                targetBuilding.NetGrid = targetNetGrid;
                CreateRoom(batchingParam, targetBuilding);
                return GetRoomInfo(batchingParam.RoomName, batchingParam.BuildingName, batchingParam.Address);//返回新建的房屋的数据
            }
            else if (targetBuilding != null && targetRoom == null)//目标楼栋存在且目标房屋不存在时: 新建该房屋
            {
                targetBuilding.NetGrid = targetNetGrid;
                CreateRoom(batchingParam, targetBuilding);
                return GetRoomInfo(batchingParam.RoomName, batchingParam.BuildingName, batchingParam.Address);//返回新建的房屋的数据
            }
            else
            {
                return new
                {
                    message = "已存在"
                };//若目标房屋已存在，返回该提示对象
            }
            
        }
        public object GetRoomInfo(string RoomName, string BuildingName, string Address)
        {
            var Room_query = from room in _context.Rooms.Where(r => r.Name == RoomName
                                                 && r.Building.Name == BuildingName
                                                 && r.Building.Address == Address)
                             select new
                             {
                                 room.Id,
                                 RoomName = room.Name,
                                 room.Category,
                                 room.Use,
                                 room.Area,
                                 room.Longitude,
                                 room.Latitude,
                                 room.Height,
                                 NetGridName = room.Building.NetGrid.Name,
                                 CommunityName = room.Building.NetGrid.Community.Name,
                                 room.Building.Address,
                                 BuildingName = room.Building.Name,
                             };
            return Room_query.ToList()[0];       
               
        }

        //单个房屋入库
        public void CreateRoom(RoomCreatingParam_Other batchingParam, Building targetBuilding)
        {
            Room newRoom = new Room
            {
                Name = batchingParam.RoomName,
                Category = batchingParam.Category,
                Use = batchingParam.Use,
                Area = batchingParam.Area,
                Longitude = batchingParam.Longitude,
                Latitude = batchingParam.Latitude,
                Height = batchingParam.Height,
            };
            newRoom.Building = targetBuilding;
            _context.Rooms.Add(newRoom);
            _context.SaveChanges();
        }
        //选择指定楼栋
        public Building PickBuilding(string BuildingName, string Address)
        {
            return _context.Buildings.SingleOrDefault(b => b.Name == BuildingName 
                                                        && b.Address == Address );
        }
        //选择指定网格
        public NetGrid PickNetGrid(string NetGridName,string CommunityName )
        {
            return _context.NetGrids.FirstOrDefault(n => n.Name == NetGridName && n.Community.Name == CommunityName);
        }
        //选择指定社区、网格、楼栋中的房屋数据
        public IEnumerable<object> GetRooms(string BuildingName, string NetGridName, string CommunityName)
        {
            return from r in _context.Rooms.Where(r => r.Building.Name == BuildingName
                                                   && r.Building.NetGrid.Name == NetGridName
                                                   && r.Building.NetGrid.Community.Name == CommunityName)
                   select new { 
                       r.Id,
                       roomName = r.Name,
                       r.Alias,
                       r.Category,
                       r.Use,
                       r.Area,
                       r.Longitude,
                       r.Latitude,
                       r.Height,
                       r.Other,
                       r.Note,
                       buildingName = r.Building.Name,
                       buildingAddress = r.Building.Address,
                       netGridName = r.Building.NetGrid.Name,
                       communityName = r.Building.NetGrid.Community.Name
                   };
        }
        //修改指定房屋信息
        public IQueryable<object> UpdateRoom(Room targetRoom, RoomCreatingParam_Other roomData)
        {
            if (targetRoom != null)
            {
                targetRoom.Name = roomData.RoomName;
                targetRoom.Category = roomData.Category;
                targetRoom.Use = roomData.Use;
                targetRoom.Area = roomData.Area;
                targetRoom.Longitude = roomData.Longitude;
                targetRoom.Latitude = roomData.Latitude;
                targetRoom.Height = roomData.Height;
            }
            _context.SaveChanges();

            return GetRoomsByBuilding(roomData.BuildingName, roomData.Address);
        }
        public IQueryable<object> DeleteRoom(Room targetRoom, RoomCreatingParam_Other roomData)
        {
            var personCount = _context.PersonRooms.Where(pr => pr.Room.Id == roomData.id).ToArray().Length;
            if (targetRoom != null && personCount == 0)
            {
                _context.Rooms.Remove(targetRoom);
                _context.SaveChanges();
            }
            return GetRoomsByBuilding(roomData.BuildingName, roomData.Address);
        }
        #endregion

        #region 姓名身份证号电话搜索
        ///<summery>
        ///通过姓名身份证号电话搜索
        /// </summery>
        public IEnumerable<object> GetPersonsBySearch(string userName, string serchName)
        {
            //通过角色返回rooms
            IQueryable<Room> rooms = FilterRoomsByRole(userName);
            if(rooms != null)
            {
                //获取rooms内所有人
                IEnumerable<IntermediatePersonRoom> data = GetroomsWithPersons(rooms);
                //根据姓名、电话、身份证过滤人
                data = FilterPersonsByRole(data, serchName);
                return GetPersonsByQueryRoom(data);
            }
            else
            {
                return GetPersonsBystreet(serchName);
            }
        
        }
        #endregion
        #region 按权限获取人房 20210726
        //第一步：前端传来的角色过滤房间
        private IQueryable<Room> FilterRoomsByRole(string userName)
        {
            var rooms = _context.Rooms.AsQueryable();
            var roleName = GetRoleName(userName);
            if (roleName == "网格员")
            {
                NetGrid netgrid = _context.NetGrids.SingleOrDefault(n => n.User.UserName == userName);
                var netidUser = netgrid.Id;
                //网格
                rooms = rooms.Where(r => r.Building.NetGrid.Id == netidUser);
                return rooms;

            }
            else
            if (roleName == "社区")
            {
                Community community = _context.Communitys.SingleOrDefault(c => c.Alias == userName);
                var communityName = community.Name;
                //社区
                rooms = rooms.Where(r => r.Building.NetGrid.Community.Name == communityName);
                return rooms;
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<object> GetPersonsBystreet(string Name)
        {
            try
            {
                //根据 room - person 数据
                var roomsWithPersons = from person in _context.Persons.Where(r => r.Name.Contains(Name) || r.PersonId == Name || r.Phone == Name)
                                       from pr in person.PersonRooms
                                       select new
                                       {
                                           RoomId = pr.Room.Id,
                                           RoomNO = pr.Room.Name,
                                           BulidingName = pr.Room.Building.Name,
                                           BulidingAddress = pr.Room.Building.Address,
                                           SubdivsionName = pr.Room.Building.Subdivision.Name,
                                           SubdivisionId = pr.Room.Building.Subdivision.Id.ToString(),
                                           CommunityName = pr.Room.Building.NetGrid.Community.Name,
                                           netGridName = pr.Room.Building.NetGrid.Name,
                                           age = pr.Person.Age,
                                           sex = pr.Person.Sex,
                                           // person.PersonId,
                                           pr.Person,
                                           IsOwner = pr.IsOwner ? "是" : "否",
                                           IsHouseholder = pr.IsHouseholder ? "是" : "否",
                                           IsLiveHere = pr.IsLiveHere ? "是" : "否",
                                           pr.RelationWithHouseholder,
                                           pr.LodgingReason,
                                           pr.PopulationCharacter
                                       };
                var psdata = roomsWithPersons.ToList();

                var data = from pr in psdata
                           join sg in _context.SpecialGroups on pr.Person.PersonId equals sg.PersonId into psg // 根据身份证关联
                           select new
                           {
                               pr.RoomId,
                               pr.RoomNO,
                               pr.netGridName,
                               pr.age,
                               pr.sex,
                               pr.Person.PersonId,
                               pr.CommunityName,
                               pr.SubdivsionName,
                               pr.BulidingName,
                               pr.BulidingAddress,
                               pr.Person,
                               pr.IsOwner,
                               pr.IsHouseholder,
                               pr.IsLiveHere,
                               pr.RelationWithHouseholder,
                               pr.LodgingReason,
                               pr.PopulationCharacter,
                               SpecialGroup = psg // 特殊人群信息
                           };
                return data;
            }
            catch (Exception e)
            {
                return null;
            }


        }
        private IEnumerable<IntermediatePersonRoom> FilterPersonsByRole(IEnumerable<IntermediatePersonRoom> roomsWithPersons, string serchName)
        {
            roomsWithPersons = roomsWithPersons.Where(pr =>pr.Person.Name.Contains(serchName) || pr.Person.PersonId == serchName || pr.Person.Phone == serchName);
            return roomsWithPersons;
        }
        #endregion
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
        public void SaveImgPath( SavepathRowParam rowdata)
        {
            Rain targetRainpoint = _context.Rains.SingleOrDefault(rp => rp.Id == rowdata.Id && rp.Name == rowdata.Name);
            if(targetRainpoint != null)
            {
                targetRainpoint.FilePath = rowdata.FilePath;
            }
            _context.SaveChanges();
        }



        //根据楼栋，返回房屋
        public IQueryable<object> GetRainById(int id)
        {
            return from b in _context.Rains.Where(b => b.Id == id)
                   select new
                   {
                       b.Id,
                       b.Name,
                       b.Longitude,
                       b.Latitude,
                       b.Height,
                       b.Report,
                       b.Status,
                       b.Type,
                       b.Address,
                       b.Note
                   };
        }

        //创建一条rain
        public IQueryable<object> CreateTargetRain()
        {
            var rainpoint = from b in _context.Rains
                            select new
                            {
                                b.Id,
                                b.Name,
                                b.Longitude,
                                b.Latitude,
                                b.Height,
                                b.Report,
                                b.Status,
                                b.Type,
                                b.Address,
                                b.Note
                            };
            return rainpoint;
        }

        public IQueryable<object> DeleteTargetRain(Rain targetRain, RainUpdateParamTesting rainFields)
        {
            if (targetRain != null)
            {
                _context.Rains.Remove(targetRain);
                _context.SaveChanges();
            }
            return GetRainById(rainFields.id);
        }

        public IQueryable<object> UpdateTargetRain()
        {
            var rainpoint = from b in _context.Rains
                            select new
                            {
                                b.Id,
                                b.Name,
                                b.Longitude,
                                b.Latitude,
                                b.Height,
                                b.Report,
                                b.Status,
                                b.Type,
                                b.Address,
                                b.Note
                            };
            return rainpoint;
        }

        public IQueryable<object> GetBoundaryStake(string kjsjbsm)
        {
            try
            {
                var countTax = (from ct in _context.BoundaryStakes.Where(cb => cb.KJSJBSM == kjsjbsm)
                                select new
                                {
                                    JZBH = ct.JZBH,
                                    KJSJBSM = ct.KJSJBSM,
                                    GC = ct.GC,
                                    ZZB = ct.ZZB,
                                    HZB = ct.HZB,
                                    JD = ct.JD,
                                    WD = ct.WD,
                                    JZDJ = ct.JZDJ,
                                    JZLX = ct.JZLX,
                                    JZCZ = ct.JZCZ
                                });
                return countTax;
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
