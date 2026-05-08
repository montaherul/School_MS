namespace SchoolManagementSystem.Helpers.Common;

public static class ReligionHelper
{
    public static string GetReligionSubjectName(string religion)
    {
        return religion switch
        {
            "Islam" => "ইসলাম ও নৈতিক শিক্ষা",
            "Hindu" => "হিন্দু ধর্ম ও নৈতিক শিক্ষা",
            "Buddhist" => "বৌদ্ধ ধর্ম ও নৈতিক শিক্ষা",
            "Christian" => "খ্রিস্টান ধর্ম ও নৈতিক শিক্ষা",
            _ => "ধর্ম ও নৈতিক শিক্ষা"
        };
    }
}
