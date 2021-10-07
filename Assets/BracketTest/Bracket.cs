using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YW;
public class NewBehaviourScript : MonoBehaviour
{
    public Column column { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<Line> Get_bracket_top_line()
    {
        var column_top_line_vertexes = Get_column_top_line_vertexes();
        var column_top_line = new List<Line>()
        {
            new Line(column_top_line_vertexes[0],column_top_line_vertexes[1]),
            new Line(column_top_line_vertexes[1],column_top_line_vertexes[2]),
            new Line(column_top_line_vertexes[2],column_top_line_vertexes[3]),
            new Line(column_top_line_vertexes[3],column_top_line_vertexes[0]),
        };
        var bracket_top_lines = new List<Line>();

        if (this.column.Girder_side.HasFlag(Directions.LEFT))
        {
            var left_outline_of_bracket = new Line(column_top_line_vertexes[0] + new Vector3(-this.column.Bracket.Thickness,0,0), column_top_line_vertexes[1] + new Vector3(-this.column.Bracket.Thickness, 0, 0));
            bracket_top_lines.Add(left_outline_of_bracket);
        }
        if (this.column.Girder_side.HasFlag(Directions.BACK))
        {
            var back_outline_of_bracket= new Line(column_top_line_vertexes[1] + new Vector3(0, 0, this.column.Bracket.Thickness), column_top_line_vertexes[2] + new Vector3(0, 0, this.column.Bracket.Thickness));
            bracket_top_lines.Add(back_outline_of_bracket);
        }
        if (this.column.Girder_side.HasFlag(Directions.RIGHT))
        {
            var right_outline_of_bracket = new Line(column_top_line_vertexes[2] + new Vector3(this.column.Bracket.Thickness,0, 0), column_top_line_vertexes[3] + new Vector3(this.column.Bracket.Thickness, 0, 0));
            bracket_top_lines.Add(right_outline_of_bracket);
        }
        if (this.column.Girder_side.HasFlag(Directions.FRONT))
        {
            var front_outline_of_bracket = new Line(column_top_line_vertexes[3] + new Vector3(0,0, -this.column.Bracket.Thickness), column_top_line_vertexes[0] + new Vector3(0, 0, -this.column.Bracket.Thickness));
            bracket_top_lines.Add(front_outline_of_bracket);
        }
        Check_intersection(bracket_top_lines);
        bracket_top_lines = Close_bracket_outlines(column_top_line, bracket_top_lines).lines;
        return bracket_top_lines;
    }

    private List<Line> Check_intersection(List<Line> bracket_top_lines)
    {
        if (bracket_top_lines.Count >= 2)
        {
            for (int i = 0; i < bracket_top_lines.Count - 1; i++)
            {
                for (int j = i + 1; j < bracket_top_lines.Count; j++)
                {
                    var first_line = bracket_top_lines[i];
                    var second_line = bracket_top_lines[j];
                    var intersection_point = new Vector3();

                    var is_intersect = Math3D.LineLineIntersection(out intersection_point,first_line.Start_point,first_line.Direction,second_line.Start_point,second_line.Direction);

                    if (is_intersect)
                    {
                        if (Vector3.Distance(first_line.Start_point, intersection_point) >= Vector2.Distance(first_line.End_point, intersection_point))
                        {
                            var new_first_line = new Line(new Vector3(first_line.Start_point.x, first_line.Start_point.y, bracket_top_lines[i].Start_point.z), new Vector3(intersection_point.x, intersection_point.y, bracket_top_lines[i].Start_point.z));
                            bracket_top_lines[i] = new_first_line;
                        }
                        else
                        {
                            var new_first_line = new Line(new Vector3(first_line.End_point.x, first_line.End_point.y, bracket_top_lines[i].Start_point.z), new Vector3(intersection_point.x, intersection_point.y, bracket_top_lines[i].Start_point.z));
                            bracket_top_lines[i] = new_first_line;
                        }
                        if (Vector2.Distance(second_line.Start_point, intersection_point) >= Vector2.Distance(second_line.End_point, intersection_point))
                        {
                            var new_second_line = new Line(new Vector3(second_line.Start_point.x, second_line.Start_point.y, bracket_top_lines[j].Start_point.z), new Vector3(intersection_point.x, intersection_point.y, bracket_top_lines[j].Start_point.z));
                            bracket_top_lines[j] = new_second_line;
                        }
                        else
                        {
                            var new_second_line = new Line(new Vector3(second_line.End_point.x, second_line.End_point.y, bracket_top_lines[j].Start_point.z), new Vector3(intersection_point.x, intersection_point.y, bracket_top_lines[j].Start_point.z));
                            bracket_top_lines[j] = new_second_line;
                        }
                    }
                }
            }
        }
        return bracket_top_lines;
    }

    private (List<Line> lines, List<Vector3> vertexes) Close_bracket_outlines(List<Line> column_outline, List<Line> bracket_outline)
    {
        var closed_bracket_outlines = bracket_outline.ToList();
        var temp_column_outline = column_outline.ToList();

        if (this.column.Girder_side.HasFlag(Directions.LEFT))
        {
            column_outline.Remove(temp_column_outline[0]);
        }
        if (this.column.Girder_side.HasFlag(Directions.BACK))
        {
            column_outline.Remove(temp_column_outline[1]);
        }
        if (this.column.Girder_side.HasFlag(Directions.RIGHT))
        {
            column_outline.Remove(temp_column_outline[2]);
        }
        if (this.column.Girder_side.HasFlag(Directions.FRONT))
        {
            column_outline.Remove(temp_column_outline[3]);
        }
        column_outline.ForEach(column_line =>
        {
            var column_start_point_vector_2 = new Vector3(column_line.Start_point.x, column_line.Start_point.y, column_line.Start_point.z);
            var column_end_point_vector_2 = new Vector3(column_line.End_point.x, column_line.Start_point.y, column_line.End_point.z);
            var column_direction_vector = column_line.End_point - column_line.Start_point;
            var column_direction_vector_2 = new Vector3(column_direction_vector.x, column_line.Start_point.y, column_direction_vector.z);

            bracket_outline.ForEach(bracket_line =>
            {
                var bracket_start_point_vector_2 = new Vector3(bracket_line.Start_point.x, bracket_line.Start_point.y, bracket_line.Start_point.z);
                var bracket_direction_vector = bracket_line.End_point - bracket_line.Start_point;
                var bracket_direction_vector_2 = new Vector3(bracket_direction_vector.x, bracket_direction_vector.y, bracket_direction_vector.z);

                var intersection_point = new Vector3();
                var is_intersect = Math3D.LineLineIntersection(out intersection_point,column_start_point_vector_2, column_direction_vector_2, bracket_start_point_vector_2, bracket_direction_vector_2);
                if (is_intersect)
                {
                    if (Vector2.Distance(column_start_point_vector_2, intersection_point) >= Vector2.Distance(column_end_point_vector_2, intersection_point))
                    {
                        var new_column_line = new Line(column_line.Start_point, new Vector3(intersection_point.x, intersection_point.y, column_line.Start_point.z));
                        closed_bracket_outlines.Add(new_column_line);
                    }
                    else
                    {
                        var new_column_line = new Line(column_line.End_point, new Vector3(intersection_point.x, intersection_point.y, column_line.End_point.z));
                        closed_bracket_outlines.Add(new_column_line);
                    }
                }
            });
        });
        var bracket_outline_vertexes = closed_bracket_outlines.Select(line => line.Start_point).ToList();
        bracket_outline_vertexes.AddRange(closed_bracket_outlines.Select(line => line.End_point));
        bracket_outline_vertexes = bracket_outline_vertexes.Distinct().OrderBy(vertex => vertex.x).ThenBy(vertex => vertex.y).ToList();
        return (closed_bracket_outlines, bracket_outline_vertexes);
    }

    private List<Vector3> Get_column_top_line_vertexes()
    {
        var column_obb = this.column.Obb;

        var column_vertexes = new List<Vector3>()
        {
            new Vector3(-column_obb.Half_width, column_obb.Half_length, -column_obb.Half_depth),
            new Vector3(-column_obb.Half_width, column_obb.Half_length, column_obb.Half_depth),
            new Vector3(column_obb.Half_width, column_obb.Half_length, column_obb.Half_depth),
            new Vector3(column_obb.Half_width, column_obb.Half_length, -column_obb.Half_depth),
        };
        column_vertexes = Align_vertexes_cw(column_vertexes);
        return column_vertexes;
    }

    private List<Vector3> Align_vertexes_cw(List<Vector3> original_vertexes)
    {
        var center_point = new Vector3(original_vertexes.Average(vertex => vertex.x), original_vertexes.Average(vertex => vertex.y), original_vertexes.Average(vertex => vertex.z));
        var ordered_vertex = original_vertexes.OrderByDescending(x => Mathf.Atan2(x.x - center_point.x, x.y - center_point.y)).ToList();
        ordered_vertex.Insert(0, ordered_vertex[3]);
        ordered_vertex.RemoveAt(4);

        return ordered_vertex;
    }


    public class Line
    {
        public Vector3 Start_point { get; private set; }
        public Vector3 End_point { get; private set; }
        public Vector3 Direction { get; private set; }

        public Line(Vector3 start_point, Vector3 end_point)
        {
            this.Start_point = start_point;
            this.End_point= end_point;
            this.Direction = end_point - start_point;
        }


        /*public static bool Check_intersection(out Vector3 intersection,Line first_line , Line second_line) 
        {
            Vector3 vector_1_dir = first_line.Start_point - first_line.End_point;
            Vector3 vector_2_dir = second_line.Start_point - second_line.End_point;
            Vector3 line_3 = second_line.Start_point = first_line.Start_point;
            Vector3 cross_vector_of_1_and_2 = Vector3.Cross(vector_1_dir, vector_2_dir);
            Vector3 cross_vector_of_3_and_2 = Vector3.Cross(line_3, vector_2_dir);

            float planar_factor = Vector3.Dot(line_3, cross_vector_of_1_and_2);

            if(Mathf.Abs(planar_factor) < 0.0001f && cross_vector_of_1_and_2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(cross_vector_of_3_and_2, cross_vector_of_1_and_2) / cross_vector_of_1_and_2.sqrMagnitude;
                intersection = first_line.Start_point + (vector_1_dir * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }*/
    }
}
