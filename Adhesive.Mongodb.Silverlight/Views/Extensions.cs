using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace Adhesive.Mongodb.Silverlight
{
    public static class Extensions
    {
        public static void ShowError(this Exception ex)
        {
            ErrorWindow error = new ErrorWindow(ex);
            error.Show();
        }

         public static IEnumerable ToDataSource(this IEnumerable<IDictionary> list)
        {

            IDictionary firstDict = null;

            bool hasData = false;

            foreach (IDictionary currentDict in list)
            {

                hasData = true;

                firstDict = currentDict;

                break;

            }

            if (!hasData)
            {

                return new object[] { };

            }

            if (firstDict == null)
            {

                throw new ArgumentException("IDictionary entry cannot be null");

            }



            Type objectType = null;



            TypeBuilder tb = GetTypeBuilder(list.GetHashCode());



            ConstructorBuilder constructor =

                        tb.DefineDefaultConstructor(

                                    MethodAttributes.Public |

                                    MethodAttributes.SpecialName |

                                    MethodAttributes.RTSpecialName);



            foreach (DictionaryEntry pair in firstDict)
            {


                    CreateProperty(tb,

                                    Convert.ToString(pair.Key),

                                    pair.Value == null ?

                                                typeof(object) :

                                                pair.Value.GetType());


            }

            objectType = tb.CreateType();



            return GenerateEnumerable(objectType, list, firstDict);

        }



        private static IEnumerable GenerateEnumerable(

                 Type objectType, IEnumerable<IDictionary> list, IDictionary firstDict)
        {

            var listType = typeof(List<>).MakeGenericType(new[] { objectType });

            var listOfCustom = Activator.CreateInstance(listType);



            foreach (var currentDict in list)
            {

                if (currentDict == null)
                {

                    throw new ArgumentException("IDictionary entry cannot be null");

                }

                var row = Activator.CreateInstance(objectType);

                foreach (DictionaryEntry pair in firstDict)
                {

                    if (currentDict.Contains(pair.Key))
                    {

                        PropertyInfo property =

                            objectType.GetProperty(Convert.ToString(pair.Key));

                        property.SetValue(

                            row,

                            Convert.ChangeType(

                                    currentDict[pair.Key],

                                    property.PropertyType,

                                    null),

                            null);

                    }

                }

                listType.GetMethod("Add").Invoke(listOfCustom, new[] { row });

            }

            return listOfCustom as IEnumerable;

        }



        private static TypeBuilder GetTypeBuilder(int code)
        {

            AssemblyName an = new AssemblyName("TempAssembly" + code);

            AssemblyBuilder assemblyBuilder =

                AppDomain.CurrentDomain.DefineDynamicAssembly(

                    an, AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");



            TypeBuilder tb = moduleBuilder.DefineType("TempType" + code

                                , TypeAttributes.Public |

                                TypeAttributes.Class |

                                TypeAttributes.AutoClass |

                                TypeAttributes.AnsiClass |

                                TypeAttributes.BeforeFieldInit |

                                TypeAttributes.AutoLayout

                                , typeof(object));

            return tb;

        }



        private static void CreateProperty(

                        TypeBuilder tb, string propertyName, Type propertyType)
        {

            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName,

                                                        propertyType,

                                                        FieldAttributes.Private);





            PropertyBuilder propertyBuilder =

                tb.DefineProperty(

                    propertyName, PropertyAttributes.HasDefault, propertyType, null);

            MethodBuilder getPropMthdBldr =

                tb.DefineMethod("get_" + propertyName,

                    MethodAttributes.Public |

                    MethodAttributes.SpecialName |

                    MethodAttributes.HideBySig,

                    propertyType, Type.EmptyTypes);



            ILGenerator getIL = getPropMthdBldr.GetILGenerator();



            getIL.Emit(OpCodes.Ldarg_0);

            getIL.Emit(OpCodes.Ldfld, fieldBuilder);

            getIL.Emit(OpCodes.Ret);



            MethodBuilder setPropMthdBldr =

                tb.DefineMethod("set_" + propertyName,

                  MethodAttributes.Public |

                  MethodAttributes.SpecialName |

                  MethodAttributes.HideBySig,

                  null, new Type[] { propertyType });



            ILGenerator setIL = setPropMthdBldr.GetILGenerator();



            setIL.Emit(OpCodes.Ldarg_0);

            setIL.Emit(OpCodes.Ldarg_1);

            setIL.Emit(OpCodes.Stfld, fieldBuilder);

            setIL.Emit(OpCodes.Ret);



            propertyBuilder.SetGetMethod(getPropMthdBldr);

            propertyBuilder.SetSetMethod(setPropMthdBldr);

        }

    }
}
