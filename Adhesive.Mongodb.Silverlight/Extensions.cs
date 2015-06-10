using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Adhesive.Mongodb.Silverlight
{
    public static class Extensions
    {
        public static string GetFilterText(this Dictionary<string, object> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dic)
                sb.AppendFormat("Key：{0}/Value：{1}，", item.Key, item.Value);
            return sb.ToString();
        }

        public static Color ToColor(this uint argb)
        {
            return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                                  (byte)((argb & 0xff0000) >> 0x10),
                                  (byte)((argb & 0xff00) >> 8),
                                  (byte)(argb & 0xff));
        }

        public static void ShowError(this Exception ex)
        {
            ErrorWindow c = new ErrorWindow(ex);

            var root = Application.Current.RootVisual as FrameworkElement;
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
        }

        public static IEnumerable ToDataSource(this IEnumerable<Dictionary<string, string>> list)
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

                 Type objectType, IEnumerable<Dictionary<string, string>> list, IDictionary firstDict)
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

                    if (currentDict.ContainsKey(pair.Key.ToString()))
                    {

                        PropertyInfo property =

                            objectType.GetProperty(Convert.ToString(pair.Key));

                        property.SetValue(

                            row,

                            Convert.ChangeType(

                                    currentDict[pair.Key.ToString()],

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
