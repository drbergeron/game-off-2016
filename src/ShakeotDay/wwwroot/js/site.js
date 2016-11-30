// Write your Javascript code.
function createDate(year, day) {

    return moment([year, 0, 1]).add(day-1, "days").format("YYYY-MM-DD");
}