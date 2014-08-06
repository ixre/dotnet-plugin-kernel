using com.mapfre.cir.Logic;
using com.mapfre.poi.ILogic;
/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 22:43
 * 
 * 修改说明：
 */
using StructureMap;

namespace com.mapfre.poi.Logic
{
    /// <summary>
    /// Description of LogicHelper.
    /// </summary>
    public class IocObject
    {
        static IocObject()
        {
            ObjectFactory.Configure(o =>
                                    {
                                        o.For<ICaseLogic>().Singleton().Use<CaseLogic>();
                                        o.For<IDataLogic>().Singleton().Use<DataLogic>();
                                    }
                                   );

            Case = ObjectFactory.GetInstance<ICaseLogic>();
            Data = ObjectFactory.GetInstance<IDataLogic>();
        }

        internal static readonly ICaseLogic Case;
        internal static readonly IDataLogic Data;
    }
}
