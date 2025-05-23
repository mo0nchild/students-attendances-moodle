import type { UserInfoWithGroupsModel } from "@core/models/user-models"
import type { CourseInfoModel } from "@models/course-models"
import { coursesService } from "@services/CoursesService"
import { AxiosError } from "axios"
import type { StateCreator } from "zustand"

export interface CourseState {
  courses: CourseInfoModel[]
  students: UserInfoWithGroupsModel[]
  isLoading: boolean
  error?: string
}

export interface CourseAction {
  fetchCourses: () => Promise<void>
  fetchStudents: (courseId: number) => Promise<void>
}

const initialCourseState: CourseState = {
  courses: [],
  students: [],
  isLoading: false,
  error: undefined
}

export type CourseSlice = CourseState & CourseAction

export const createCourseSlice: StateCreator<CourseSlice, [], [], CourseSlice> = (set) => ({
  fetchCourses: async () => {
    try {
			set({ error: undefined, isLoading: true })
			const coursesList = (await coursesService.getCoursesList()).data

    	set({ isLoading: false, courses: coursesList })
    }
    catch (error) {
      if (error instanceof AxiosError) {
        set({ 
          ...initialCourseState, 
          error: error.response ? 'Невозможно получить курсы' : error.message
        })
      } else {
        set({ ...initialCourseState, error: 'Неизвестная ошибка' })
      }
			console.log(error)
    }
  },
  fetchStudents: async (courseId: number) => {
    try {
      set({ error: undefined, isLoading: true })
      const studentsList = (await coursesService.getStudentsList(courseId)).data

      set({ isLoading: false, students: studentsList })
    } 
    catch (error) {
      if (error instanceof AxiosError) {
        set({ 
          ...initialCourseState, 
          error: error.response ? 'Невозможно получить студентов' : error.message
        })
      } else {
        set({ ...initialCourseState, error: 'Неизвестная ошибка' })
      }
			console.log(error)
    }
  },
  ...initialCourseState
})