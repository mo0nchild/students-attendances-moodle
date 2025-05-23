import type { CourseInfoModel } from "@models/course-models";
import { Badge, Button, Table } from "react-bootstrap";

import React from 'react';

import './CourseTable.css'
import { BadgeInfoIcon, CalendarDays } from "lucide-react";


interface Props {
  courses: CourseInfoModel[];
  onSelect?: (id: number) => void;
  onLesson?: (id: number) => void;
}

const getBadgeColor = (groupMode: string): string => {
  switch (groupMode) {
    case 'None': return 'secondary';
    case 'Isolated': return 'primary';
    case 'Visible': return 'success';
    default: return 'dark';
  }
};

const CourseTable: React.FC<Props> = ({ courses, onSelect, onLesson }) => {
  return (
    <div className="rounded shadow-sm p-3 bg-white bg-opacity-75" style={{
      overflow: 'auto'
    }}>
      <Table responsive striped className="align-middle mb-0 w-100" style={{
        backgroundColor: 'transparent',
        overflow: 'auto'
      }}>
        <thead style={{fontSize: '15px', fontWeight: 'normal'}}>
          <tr>
            <th>#</th>
            <th style={{width: '100px'}}>Название</th>
            <th style={{width: '400px'}}>Полное название</th>
            <th style={{width: '200px'}}>Период</th>
            <th style={{width: '200'}}>Модули</th>
            <th style={{width: 'auto'}}></th>
            <th style={{width: 'auto'}}></th>
          </tr>
        </thead>
        <tbody style={{fontSize: '14px'}}>
          {courses.map((course, index) => (
            <tr key={course.externalId}>
              <td>{index + 1}</td>
              <td>{course.shortName}</td>
              <td>{course.fullName}</td>
              <td>{new Date(course.startDate).toLocaleDateString()} 
                - {course.endDate ? new Date(course.endDate).toLocaleDateString() : '-'}</td>
              <td>
                {course.attendanceModules.length > 0 ? (
                  <ul className="mb-0 ps-3 list-unstyled">
                    {course.attendanceModules.map((mod) => (
											<li key={mod.externalId} className="mb-1">
                        {mod.name}{' '}
                        <Badge pill bg={getBadgeColor(mod.groupMode)} className="ms-1">
                            {mod.groupMode}
                        </Badge>
											</li>
										))}
									</ul>
                ) : (
                  <span className="text-muted">None</span>
                )}
              </td>
              <td>
                <Button style={{backgroundColor: '#FFFFFF', borderColor: 'grey'}} 
                  onClick={() => onSelect?.(course.externalId)}  
                >
                  <BadgeInfoIcon size={15} color="#000"/>
                </Button>
              </td>
              <td>
                <Button style={{backgroundColor: '#FFFFFF', borderColor: 'grey'}} 
                  onClick={() => onLesson?.(course.externalId)}  
                >
                  <CalendarDays size={15} color="#000"/>
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default CourseTable;