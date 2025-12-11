using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Seed
{
    public static class DataSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HemaBazaarDBContext>();

            // ================================
            // 1) CATEGORIES
            // ================================
            if (!await dbContext.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { CategoryName = "Kılıçlar",       IsActive = true, CreatedDate = DateTime.Now },
                    new Category { CategoryName = "Kalkanlar",      IsActive = true, CreatedDate = DateTime.Now },
                    new Category { CategoryName = "Zırhlar",        IsActive = true, CreatedDate = DateTime.Now },
                    new Category { CategoryName = "Kasklar",        IsActive = true, CreatedDate = DateTime.Now },
                    new Category { CategoryName = "HEMA Ekipmanı",  IsActive = true, CreatedDate = DateTime.Now },
                    new Category { CategoryName = "Aksesuarlar",    IsActive = true, CreatedDate = DateTime.Now }
                };

                await dbContext.Categories.AddRangeAsync(categories);
                await dbContext.SaveChangesAsync();
            }

            // NOT: Boş tabloda identity 1'den başlayacağı için:
            // 1 = Kılıçlar, 2 = Kalkanlar, 3 = Zırhlar, 4 = Kasklar, 5 = HEMA, 6 = Aksesuarlar
            // Aşağıdaki CategoryId'ler buna göre ayarlı.

            // ================================
            // 2) ITEMS
            // ================================
            if (!await dbContext.Items.AnyAsync())
            {
                var items = new List<Item>
                {
                    // ================================
                    // KILIÇLAR (CategoryId = 1)
                    // ================================
                    new Item {
                        Title = "Arming Sword",
                        Description = "Ortaçağ tek elli bir kılıç. Dengeli, hafif ve hızlı.",
                        Content = "1060 karbon çelikten üretilmiş, tam saptamalı (full tang), el yapımı arming sword.",
                        Price = 3500,
                        CategoryId = 1,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Longsword",
                        Description = "HEMA’ya uygun uzun kılıç. Mükemmel denge.",
                        Content = "Kesici olmayan eğitim longsword. Çelik karışımı esnektir.",
                        Price = 4200,
                        CategoryId = 1,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Viking Sword",
                        Description = "Geniş yaprak tasarımına sahip erken dönem kılıcı.",
                        Content = "EN45 çelik blade, deri sarılı sap, geniş fuller tasarımı.",
                        Price = 3900,
                        CategoryId = 1,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Katana",
                        Description = "Geleneksel Japon savaş kılıcı.",
                        Content = "1060 karbon çelik, saya dahil, hamon çizgisi mevcut.",
                        Price = 6000,
                        CategoryId = 1,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Gladius",
                        Description = "Roma lejyoner kısa kılıcı.",
                        Content = "Pompeii tipi gladius, ahşap kabza.",
                        Price = 3000,
                        CategoryId = 1,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Scimitar",
                        Description = "Osmanlı tarzı eğimli kılıç.",
                        Content = "Kavisli çelik, pirinç işlemeli kabza.",
                        Price = 5200,
                        CategoryId = 1,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },

                    // ================================
                    // KALKANLAR (CategoryId = 2)
                    // ================================
                    new Item {
                        Title = "Round Shield",
                        Description = "Viking yuvarlak kalkan.",
                        Content = "Ahşap gövde, deri kaplama, metal boss.",
                        Price = 2800,
                        CategoryId = 2,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Heater Shield",
                        Description = "Ortaçağ şövalye kalkanı.",
                        Content = "Bez kaplama, boyanabilir yüzey.",
                        Price = 3500,
                        CategoryId = 2,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Kite Shield",
                        Description = "Norman uzun kalkanı.",
                        Content = "Uzun gövde yapısı ile tam koruma sağlar.",
                        Price = 3300,
                        CategoryId = 2,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Buckler",
                        Description = "Küçük ve hızlı el kalkanı.",
                        Content = "1.5mm çelik, hafif kullanım.",
                        Price = 1200,
                        CategoryId = 2,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Tower Shield",
                        Description = "Roma scutum benzeri büyük kalkan.",
                        Content = "Kavisli yapılı, yüksek koruma sağlar.",
                        Price = 4800,
                        CategoryId = 2,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Targe Shield",
                        Description = "İskoç dağcı kalkanı.",
                        Content = "Nubuk deri kaplama ve metal süslemeler.",
                        Price = 2900,
                        CategoryId = 2,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },

                    // ================================
                    // ZIRHLAR (CategoryId = 3)
                    // ================================
                    new Item {
                        Title = "Chainmail Hauberk",
                        Description = "Tam boy zincir zırh.",
                        Content = "8mm halkalı, butted örgü.",
                        Price = 6500,
                        CategoryId = 3,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Brigandine",
                        Description = "Perçinli plakalı deri zırh.",
                        Content = "İçte çelik plakalar, dışta kaliteli deri.",
                        Price = 7800,
                        CategoryId = 3,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Plate Armor Chest",
                        Description = "Göğüs plakası.",
                        Content = "2mm çelik, ayarlanabilir kayışlar.",
                        Price = 9200,
                        CategoryId = 3,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Gambeson",
                        Description = "Vatkalı zırh altlığı.",
                        Content = "Çok katmanlı pamuk dolgu.",
                        Price = 2300,
                        CategoryId = 3,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Arm Armor Set",
                        Description = "Kol zırh seti.",
                        Content = "Dirsek ve kol plakaları dahil.",
                        Price = 3500,
                        CategoryId = 3,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Leg Armor Set",
                        Description = "Bacak zırh seti.",
                        Content = "Dizlik ve kaval seti.",
                        Price = 3600,
                        CategoryId = 3,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },

                    // ================================
                    // KASKLAR (CategoryId = 4)
                    // ================================
                    new Item {
                        Title = "Nasal Helmet",
                        Description = "Viking burun korumalı kask.",
                        Content = "2mm çelik, deri iç dolgu.",
                        Price = 2800,
                        CategoryId = 4,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Great Helm",
                        Description = "Haçlı şövalyesi kaskı.",
                        Content = "1.8 mm çelik plaka.",
                        Price = 4500,
                        CategoryId = 4,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Sallet Helmet",
                        Description = "Alman stili kuyruğu olan kask.",
                        Content = "Geniş yüz koruması, dar visor.",
                        Price = 3900,
                        CategoryId = 4,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Barbute",
                        Description = "Antik Yunan esintili yüz açıklığı geniş kask.",
                        Content = "2mm çelik üretim.",
                        Price = 3200,
                        CategoryId = 4,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Bascinet",
                        Description = "HEMA turnuva kaskı.",
                        Content = "Kafes vizörlü.",
                        Price = 5200,
                        CategoryId = 4,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Kabuto",
                        Description = "Samuray zırh kaskı.",
                        Content = "Lame plakalı, dekoratif ön süsleme.",
                        Price = 6000,
                        CategoryId = 4,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },

                    // ================================
                    // HEMA EKİPMANI (CategoryId = 5)
                    // ================================
                    new Item {
                        Title = "HEMA Feder",
                        Description = "Turnuvaya uygun feder.",
                        Content = "Esnek çelik, güvenli uç tasarımı.",
                        Price = 3500,
                        CategoryId = 5,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "HEMA Mask",
                        Description = "1600N fencing mask.",
                        Content = "Boyun koruması takılabilir.",
                        Price = 1700,
                        CategoryId = 5,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "HEMA Gloves",
                        Description = "Dayanıklı HEMA eldiveni.",
                        Content = "Kırılma önleyici sert plakalar.",
                        Price = 1400,
                        CategoryId = 5,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Gorget",
                        Description = "Boyun koruması.",
                        Content = "Deri + çelik karışımı.",
                        Price = 700,
                        CategoryId = 5,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "HEMA Jacket",
                        Description = "350N HEMA ceketi.",
                        Content = "Darbelere dayanıklı, nefes alır yapı.",
                        Price = 2600,
                        CategoryId = 5,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Padded Shorts",
                        Description = "Koruyucu HEMA short.",
                        Content = "Kalça ve uyluk bölgesini korur.",
                        Price = 1300,
                        CategoryId = 5,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },

                    // ================================
                    // AKSESUARLAR (CategoryId = 6)
                    // ================================
                    new Item {
                        Title = "Leather Belt",
                        Description = "Kılıç taşıma kemeri.",
                        Content = "Gerçek deri üretim.",
                        Price = 350,
                        CategoryId = 6,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Sheath",
                        Description = "Kılıç kını.",
                        Content = "Deri kaplama.",
                        Price = 600,
                        CategoryId = 6,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Chainmail Coif",
                        Description = "Zincir zırh başlık.",
                        Content = "8mm butted halkalar.",
                        Price = 1200,
                        CategoryId = 6,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Medieval Pouch",
                        Description = "Deri kemer kesesi.",
                        Content = "El yapımı deri doku.",
                        Price = 450,
                        CategoryId = 6,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Arm Bracers",
                        Description = "Deri kol koruyucu.",
                        Content = "Siyah deri + metal perçin.",
                        Price = 900,
                        CategoryId = 6,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Item {
                        Title = "Cape",
                        Description = "Ortaçağ pelerini.",
                        Content = "Yünlü kumaş, bağcıklı yaka.",
                        Price = 800,
                        CategoryId = 6,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                };

                await dbContext.Items.AddRangeAsync(items);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
