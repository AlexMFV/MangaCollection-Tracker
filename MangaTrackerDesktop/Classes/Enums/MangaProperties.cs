using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaTrackerDesktop
{
    public enum MangaProperties
    {
        ID,
        GID,
        TYPE,
        NAME,
        PRECISION,
        VINTAGE,
        INFO, 
        NEWS,
        RELEASE,
        RATINGS,
        STAFF,
        NONE
    }

    public enum MangaPropertyInfo
    {
        PICTURE,
        ALT_TITLE,
        GENRES,
        PLOTSUMMARY,
        //AUTHOR_ID, //Or Person_id
        NONE
    }
}
