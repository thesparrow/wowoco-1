# 1 -- DbContext.cs

ADD CONSTRUCTOR == How a class will be invoked in a program.

public DbBridge() {}
  () == we're not sending info to it

IE: public DbBridge(x) {}


=================================
public class DbBridge : DbContext
    {
        // WOWOCO: DEFAULT CONSTRUCTOR (JUST DO THIS!)
        public DbBridge() { }

        // WOWOCO: CONSTRUCTOR (JUST DO THIS!)
        public DbBridge(DbContextOptions<DbBridge> options) : base(options) { }

        // WOWOCO: TABLE IN THE DATABASE; EACH TABLE GETS ITS OWN LINE.
        // public DbSet<ENTER-TABLENAME-HERE> ENTER-TABLENAME-HERE { get; set; }
        public DbSet<Greetings> Greetings { get; set; }

    }

=================================



2 -- Startup.cs

LET SERVICES KNOW ABOUT YOUR DATABASE

==================================
services.AddDbContext<DbBridge>(options => options.UseSqlite(Configuration["MyDB"]));
==================================


3 -- appsettings.json
==================================
"MyDB": "Filename=/db/mydatabase.db",
==================================

4 -- Startup.cs
==================================
            using (var serviceScope = app
                .ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                serviceScope
                    .ServiceProvider
                    .GetService<DbBridge>()
                    .Database
                    .EnsureCreated();
            }
==================================






                        // DB Related Customized values added with each record
                        //_myGreetings.CreateDate = DateTime.Now.ToString();
                        //_myGreetings.CreateIP = this.HttpContext.Connection.RemoteIpAddress.ToString();

                        //Clean Data before insertion 
                        _myGreetings.senderemail = _myGreetings.senderemail.ToLowerInvariant();
                        _myGreetings.friendemail = _myGreetings.friendemail.ToLowerInvariant();

                        // DB Related add record






